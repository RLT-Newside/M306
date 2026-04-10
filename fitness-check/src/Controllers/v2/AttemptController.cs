using System.Numerics;
using AutoMapper;
using FitnessCheck.Data;
using FitnessCheck.Data.DTO.Response;
using FitnessCheck.Data.Entities;
using FitnessCheck.Services;
using FitnessCheck.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FitnessCheck.Controllers.v2;

/// <summary>
/// Abstract base controller for v2 fitness check attempts.
/// Provides shared logic for discipline-specific controllers.
/// </summary>
public abstract class AttemptController(
    FitnessCheckDbContext _dbContext,
    IMapper _mapper,
    IGenderService _genderService,
    ICohortService _cohortService,
    IBestAttemptService _bestAttemptService,
    IAttemptService attemptService,
    IOptions<MaxAllowedAttemptsOptions> maxAllowedAttempts
) : Controller
{
    /// <summary>
    /// Gets the maximum allowed attempts configuration options.
    /// </summary>
    protected readonly MaxAllowedAttemptsOptions _maxAllowedAttemptsOptions = maxAllowedAttempts.Value;

    /// <summary>
    /// Service for managing fitness check attempts.
    /// </summary>
    protected readonly IAttemptService _attemptService = attemptService;

    /// <summary>
    /// Retrieves user data including user ID, username, gender, and cohort information.
    /// </summary>
    /// <returns>A tuple containing user ID, username, gender, and cohort.</returns>
    protected async Task<(Guid, string, char, Cohort)> GetUserDataAsync()
    {
        var userId = UserClaimsUtils.GetUserId(User);
        var username = UserClaimsUtils.GetUsername(User);
        var userData = (
            userId,
            username,
            await _genderService.GetGenderAsync(username),
            (await _cohortService.GetCohortAsync(username))!
        );
        return userData;
    }

    /// <summary>
    /// Gets the attempt response for a specific discipline, including attempts, remaining attempts, 
    /// points, cohort averages, and annual performance data.
    /// </summary>
    /// <typeparam name="TDiscipline">The discipline attempt entity type.</typeparam>
    /// <typeparam name="TResultValue">The result value type (e.g., uint, float).</typeparam>
    /// <typeparam name="TResponseDTO">The response DTO type for the discipline.</typeparam>
    /// <param name="schoolYear">Optional school year to filter attempts. Defaults to current school year.</param>
    /// <returns>An action result containing the attempts response DTO.</returns>
    protected async Task<ActionResult<AttemptsResponseDTO<TResponseDTO, TResultValue>>> GetAttemptResponse<TDiscipline, TResultValue, TResponseDTO>(ushort? schoolYear = null)
        where TResultValue : INumber<TResultValue>
        where TDiscipline : DisciplineAttempt<TResultValue>, new()
        where TResponseDTO : DisciplineAttemptResponseDTO
    {
        var userId = UserClaimsUtils.GetUserId(User);

        var schoolYearStart = SchoolYearUtils.GetSchoolYearStart(schoolYear);
        var schoolYearEnd = schoolYearStart.AddYears(1).AddSeconds(-1);

        var allAttempts = await _dbContext.Set<TDiscipline>()
                                          .Where(attempt => attempt.UserId == userId)
                                          .OrderBy(attempt => attempt.AttemptNumber)
                                          .ToListAsync();

        // Filter attempts for school year
        var schoolYearAttempts = allAttempts.Where(attempt => attempt.MomentUtc >= schoolYearStart && attempt.MomentUtc <= schoolYearEnd)
                                            .ToList();

        var maxAllowedAttempts = _maxAllowedAttemptsOptions.GetForDisciplineAttempt<TDiscipline, TResultValue>();
        uint remainingAttempts;

        if (typeof(TDiscipline) == typeof(OneLegStandAttempt))
        {
            // For the one leg stand, the maximum allowed attempts are defined PER LEG
            maxAllowedAttempts *= 2;

            // Count attempts per foot in current school year
            var leftFootAttempts = schoolYearAttempts.Count(a => (a as OneLegStandAttempt)!.Foot == EFoot.Left);
            var rightFootAttempts = schoolYearAttempts.Count(a => (a as OneLegStandAttempt)!.Foot == EFoot.Right);

            // Calculate remaining attempts considering both feet
            remainingAttempts = (uint)(maxAllowedAttempts - (leftFootAttempts + rightFootAttempts));
        }
        else
        {
            remainingAttempts = (uint)(maxAllowedAttempts - schoolYearAttempts.Count);
        }

        // Assumption: There's a single cohort for all attempts of a single user throughout a school year.
        var cohortId = schoolYearAttempts.Select(attempt => attempt.CohortId).FirstOrDefault();
        CohortAverageResponseDTO<TResultValue>? cohortAverage = default;
        if (!cohortId.Equals(Guid.Empty))
        {
            cohortAverage = await GetCohortAverage<TDiscipline, TResultValue>(cohortId);
        }

        var bestAttempt = await _dbContext.BestAttempts.FirstOrDefaultAsync(entry => entry.UserId == userId
                                                                                     && entry.CohortId == cohortId
                                                                                     && entry.Discipline == typeof(TDiscipline).Name.Replace("Attempt", ""));

        var attemptDTOs = _mapper.Map<IEnumerable<TDiscipline>, List<TResponseDTO>>(schoolYearAttempts);

        // Get gender from first attempt if available, otherwise from gender service
        char gender;
        if (schoolYearAttempts.Count != 0)
        {
            gender = schoolYearAttempts.First().Gender;
        }
        else
        {
            var username = UserClaimsUtils.GetUsername(User);
            gender = await _genderService.GetGenderAsync(username);
        }

        // AnnualPerformance: for each school year up to and including the current, get the user's best attempt for this discipline
        var annualPerformance = await _bestAttemptService.GetAnnualPerformanceAsync<TDiscipline>(userId);

        var currentSchoolYear = SchoolYearUtils.GetSchoolYearForDate();
        var year = schoolYear ?? SchoolYearUtils.GetSchoolYearForDate();
        var response = new AttemptsResponseDTO<TResponseDTO, TResultValue>()
        {
            Attempts = attemptDTOs,
            RemainingAttempts = remainingAttempts,
            MaxAllowedAttempts = maxAllowedAttempts,
            Points = bestAttempt?.Points ?? 0,
            CohortAverageResult = cohortAverage?.AverageResult,
            CohortAveragePoints = cohortAverage?.AveragePoints,
            AnnualPerformance = annualPerformance,
            SchoolYear = year,
            IsCurrentSchoolYear = year == currentSchoolYear,
            Gender = gender,
        };

        return Ok(response);
    }

    /// <summary>
    /// Calculates the cohort average for a specific discipline based on best attempts.
    /// </summary>
    /// <typeparam name="TDiscipline">The discipline attempt entity type.</typeparam>
    /// <typeparam name="TResultValue">The result value type (e.g., uint, float).</typeparam>
    /// <param name="cohortId">The cohort ID to calculate averages for.</param>
    /// <returns>A cohort average response DTO containing average result and points.</returns>
    protected async Task<CohortAverageResponseDTO<TResultValue>> GetCohortAverage<TDiscipline, TResultValue>(Guid cohortId)
        where TDiscipline : DisciplineAttempt
        where TResultValue : INumber<TResultValue>
    {
        // Get all BestAttempt entities for the same cohort and discipline
        var disciplineName = typeof(TDiscipline).Name.Replace("Attempt", "");
        var bestAttemptsByUser = await _dbContext.BestAttempts
            .Where(b => b.CohortId == cohortId && b.Discipline == disciplineName)
            .ToListAsync();

        // Cannot calculate cohort average if no best attempts are available.
        if (bestAttemptsByUser.Count == 0) return default!;

        // Calculate average for actual (metric) result as float for fractional values.
        float averageResult = bestAttemptsByUser
            .Select(bestAttempt => bestAttempt.Result)
            .Average();

        // Calculate average for the best attempts points.
        var averagePoints = bestAttemptsByUser
            .Select(b => (float)b.Points)
            .Average();

        return new CohortAverageResponseDTO<TResultValue>
        {
            AverageResult = averageResult,
            AveragePoints = averagePoints
        };
    }
}
