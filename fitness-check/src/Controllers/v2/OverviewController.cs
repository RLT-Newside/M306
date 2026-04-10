using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessCheck.Data;
using FitnessCheck.Data.Entities;
using FitnessCheck.Utils;
using Microsoft.AspNetCore.Authorization;
using FitnessCheck.Data.DTO.Response;
using AutoMapper;
using FitnessCheck.Services;

namespace FitnessCheck.Controllers.v2;

/// <summary>
/// API controller for retrieving a yearly overview of the authenticated user's best attempts and cohort data.
/// </summary>
[ApiController]
[Route("v2/[controller]")]

public class OverviewController(FitnessCheckDbContext _dbContext, IMapper _mapper, ICohortService _cohortService) : ControllerBase
{
    /// <summary>
    /// Returns a yearly overview of the authenticated user's best attempts and cohort information.
    /// </summary>
    /// <remarks>
    /// Each entry in the result represents a school year, including the user's best attempts and cohort details for that year.
    /// </remarks>
    /// <returns>A list of yearly overviews, one for each school year.</returns>
    /// <response code="200">Returns the yearly overview for the authenticated user.</response>
    /// <response code="401">If the user is not authenticated.</response>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<YearOverviewResponseDTO>), 200)]
    [ProducesResponseType(401)]
    [Produces("application/json")]
    public async Task<ActionResult<IEnumerable<YearOverviewResponseDTO>>> GetOverview()
    {
        var userId = UserClaimsUtils.GetUserId(User);

        // Get all best attempts for this user
        var bestAttempts = await _dbContext.BestAttempts
            .Where(b => b.UserId == userId)
            .ToListAsync();

        // Group by school year
        var attemptsByYear = bestAttempts
            .GroupBy(b => SchoolYearUtils.GetSchoolYearForDate(b.MomentUtc))
            .ToDictionary(g => g.Key, g => g.ToList());

        // Always include the current school year
        var currentYear = SchoolYearUtils.GetSchoolYearForDate();
        if (!attemptsByYear.ContainsKey(currentYear))
        {
            attemptsByYear[currentYear] = [];
        }

        var overviewList = new List<YearOverviewResponseDTO>();
        foreach (var yearAttempts in attemptsByYear.OrderBy(x => x.Key))
        {
            var year = yearAttempts.Key;
            var attempts = yearAttempts.Value;
            var yearOverview = await CreateYearOverviewAsync(year, attempts, currentYear);
            overviewList.Add(yearOverview);
        }

        return Ok(overviewList);
    }

    /// <summary>
    /// Returns a yearly overview of the authenticated user's best attempts and cohort information for a specific year.
    /// </summary>
    /// <param name="year">The school year (e.g., 2025).</param>
    /// <returns>The yearly overview for the specified year.</returns>
    /// <response code="200">Returns the yearly overview for the specified year.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="404">If no data is found for the specified year.</response>
    [HttpGet("{year}")]
    [Authorize]
    [ProducesResponseType(typeof(YearOverviewResponseDTO), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    [Produces("application/json")]
    public async Task<ActionResult<YearOverviewResponseDTO>> GetYearOverview(ushort year)
    {
        var userId = UserClaimsUtils.GetUserId(User);

        // Get all best attempts for this user for the specified year
        var bestAttempts = (await _dbContext.BestAttempts
            .Where(b => b.UserId == userId)
            .ToListAsync())
            .Where(b => SchoolYearUtils.GetSchoolYearForDate(b.MomentUtc) == year)
            .ToList();

        var currentYear = SchoolYearUtils.GetSchoolYearForDate();
        var yearOverview = await CreateYearOverviewAsync(year, bestAttempts, currentYear);

        // For past years with no attempts and no cohort, return 404
        if (bestAttempts.Count == 0 && year != currentYear && yearOverview.Cohort == null)
        {
            return NotFound($"No data found for year {year}");
        }

        return Ok(yearOverview);
    }

    /// <summary>
    /// Retrieves the cohort for a given year and list of attempts.
    /// </summary>
    /// <param name="year">The school year.</param>
    /// <param name="attempts">The list of best attempts for the year.</param>
    /// <param name="currentYear">The current school year.</param>
    /// <returns>The cohort entity or null if not found.</returns>
    private async Task<Cohort?> GetCohortForYearAsync(ushort year, List<BestAttempt> attempts, ushort currentYear)
    {
        if (attempts.Count > 0)
        {
            // Use the first attempt's CohortId to find the cohort
            var cohortId = attempts[0].CohortId;
            return await _dbContext.Cohorts.FirstOrDefaultAsync(c => c.Id == cohortId);
        }
        else if (year == currentYear)
        {
            // Only for the current school year, get the cohort from the service
            var username = UserClaimsUtils.GetUsername(User);
            return await _cohortService.GetCohortAsync(username);
        }

        return null;
    }

    /// <summary>
    /// Creates a year overview DTO from the given attempts and year information.
    /// </summary>
    /// <param name="year">The school year.</param>
    /// <param name="attempts">The list of best attempts for the year.</param>
    /// <param name="currentYear">The current school year.</param>
    /// <returns>The year overview DTO.</returns>
    private async Task<YearOverviewResponseDTO> CreateYearOverviewAsync(ushort year, List<BestAttempt> attempts, ushort currentYear)
    {
        var bestAttemptDtos = _mapper.Map<List<BestAttemptResponseDTO>>(attempts);
        float? avgPoints = attempts.Count > 0 ? (float?)attempts.Average(a => a.Points) : null;

        var cohort = await GetCohortForYearAsync(year, attempts, currentYear);
        var cohortDto = cohort != null ? _mapper.Map<CohortResponseDTO>(cohort) : null;

        return new YearOverviewResponseDTO
        {
            SchoolYear = year,
            BestAttempts = bestAttemptDtos,
            TotalPoints = attempts.Count > 0 ? (uint?)attempts.Sum(a => a.Points) : null,
            AveragePoints = avgPoints,
            Rating = avgPoints is null ? null : GradingUtils.GetRating((uint)avgPoints),
            Cohort = cohortDto!
        };
    }
}
