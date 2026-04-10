using FitnessCheck.Data;
using FitnessCheck.Data.Entities;
using FitnessCheck.Data.DTO.Response;
using Microsoft.EntityFrameworkCore;

namespace FitnessCheck.Services;
/// <summary>
/// Service for calculating and retrieving best attempts and annual performance for fitness check disciplines.
/// </summary>
public class BestAttemptService(FitnessCheckDbContext _dbContext, IPointsCalculator _pointsCalculator) : IBestAttemptService
{
    /// <inheritdoc/>
    public async Task<List<AnnualPerformanceResponseDTO>> GetAnnualPerformanceAsync<TDiscipline>(Guid userId)
        where TDiscipline : DisciplineAttempt
    {
        var annualPerformance = new List<AnnualPerformanceResponseDTO>();
        
        // Get the overall year range across ALL disciplines for this user
        var firstAttemptOverall = await _dbContext.BestAttempts
            .Where(b => b.UserId == userId)
            .OrderBy(b => b.MomentUtc)
            .FirstOrDefaultAsync();

        if (firstAttemptOverall != null)
        {
            var firstYear = Utils.SchoolYearUtils.GetSchoolYearForDate(firstAttemptOverall.MomentUtc);
            var currentYear = Utils.SchoolYearUtils.GetSchoolYearForDate();
            var disciplineName = typeof(TDiscipline).Name.Replace("Attempt", "");

            for (ushort year = firstYear; year <= currentYear; year++)
            {
                var yearStart = Utils.SchoolYearUtils.GetSchoolYearStart(year);
                var yearEnd = yearStart.AddYears(1).AddSeconds(-1);

                var best = await _dbContext.BestAttempts
                    .Where(b => b.UserId == userId && b.Discipline == disciplineName && b.MomentUtc >= yearStart && b.MomentUtc <= yearEnd)
                    .OrderByDescending(b => b.Points)
                    .FirstOrDefaultAsync();

                // Add an entry for every year, with null values if no best attempt exists
                annualPerformance.Add(new AnnualPerformanceResponseDTO
                {
                    Year = year,
                    Result = best?.Result,
                    Points = best?.Points
                });
            }
        }
        return annualPerformance;
    }

    /// <inheritdoc/>
    public async Task<BestAttempt?> CalculateBestAttemptAsync<TDiscipline, TResultValue>(Guid userId, Guid cohortId)
        where TDiscipline : DisciplineAttempt<TResultValue>, new()
        where TResultValue : System.Numerics.INumber<TResultValue>
    {
        // Get the discipline name (e.g., "CoreStrength", "ShuttleRun", etc.)
        var disciplineName = typeof(TDiscipline).Name.Replace("Attempt", "");

        // Special handling for OneLegStand: sum the best left and right foot results
        if (typeof(TDiscipline) == typeof(OneLegStandAttempt))
        {
            // Get the best left foot attempt for this user and cohort
            var left = await _dbContext.Set<OneLegStandAttempt>()
                .Where(a => a.UserId == userId && a.CohortId == cohortId && a.Foot == EFoot.Left)
                .OrderByDescending(a => a.ResultInSeconds)
                .FirstOrDefaultAsync();
            // Get the best right foot attempt for this user and cohort
            var right = await _dbContext.Set<OneLegStandAttempt>()
                .Where(a => a.UserId == userId && a.CohortId == cohortId && a.Foot == EFoot.Right)
                .OrderByDescending(a => a.ResultInSeconds)
                .FirstOrDefaultAsync();

            // Use 0 if no attempt exists for a foot
            var leftResult = left?.ResultInSeconds ?? 0;
            var rightResult = right?.ResultInSeconds ?? 0;
            // The sum of both feet is the result
            var sumResult = leftResult + rightResult;
            // Use the available gender, or 'x' if unknown
            var gender = left?.Gender ?? right?.Gender ?? 'x';

            // Calculate points for the summed result
            var pointsFilter = new OneLegStandAttempt().GetPointsFilter(sumResult, gender);
            var points = _pointsCalculator.CalculatePoints(pointsFilter);

            // Determine the latest attempt moment for OneLegStand (either foot)
            DateTime? leftMoment = left?.MomentUtc;
            DateTime? rightMoment = right?.MomentUtc;
            DateTime momentUtc = (leftMoment, rightMoment) switch
            {
                (DateTime l, DateTime r) => l > r ? l : r,
                (DateTime l, null) => l,
                (null, DateTime r) => r,
                _ => DateTime.MinValue
            };

            // Return a denormalized BestAttempt entity for OneLegStand
            return new BestAttempt
            {
                UserId = userId,
                CohortId = cohortId,
                Discipline = disciplineName,
                LeftFootResult = leftResult,
                RightFootResult = rightResult,
                Result = sumResult,
                Points = points,
                MomentUtc = momentUtc,
                LastCalculated = DateTime.UtcNow
            };
        }
        else
        {
            // For all other disciplines, get the best attempt (highest or lowest depending on discipline)
            IEnumerable<TDiscipline> ordered;
            if (typeof(TDiscipline) == typeof(ShuttleRunAttempt))
            {
                // For ShuttleRun, a lower value is better (e.g., time)
                ordered = _dbContext.Set<TDiscipline>()
                    .Where(a => a.UserId == userId && a.CohortId == cohortId)
                    .AsEnumerable()
                    .OrderBy(a => a.GetResultValue());
            }
            else
            {
                // For all other disciplines, a higher value is better
                ordered = _dbContext.Set<TDiscipline>()
                    .Where(a => a.UserId == userId && a.CohortId == cohortId)
                    .AsEnumerable()
                    .OrderByDescending(a => a.GetResultValue());
            }

            // Get the best entity (if any)
            var bestEntity = ordered.FirstOrDefault();

            if (bestEntity == null) return null;

            // Extract the result and gender
            var resultValue = bestEntity.GetResultValue();
            var gender = bestEntity.Gender;

            // Calculate points for this result
            var pointsFilter = bestEntity.GetPointsFilter(resultValue, gender);
            var points = _pointsCalculator.CalculatePoints(pointsFilter);

            // Return a denormalized BestAttempt entity for this discipline
            return new BestAttempt
            {
                UserId = userId,
                CohortId = cohortId,
                Discipline = disciplineName,
                Result = Convert.ToSingle(resultValue),
                Points = points,
                MomentUtc = bestEntity.MomentUtc,
                LastCalculated = DateTime.UtcNow
            };
        }
    }

    /// <inheritdoc/>
    public async Task<List<BestAttempt>> GetAllBestAttemptsAsync(Guid userId, Guid cohortId)
    {
        var bestAttempts = new List<BestAttempt>();
        // List all discipline types and their result types
        var disciplineTypePairs = new (Type type, Type resultType)[]
        {
                (typeof(CoreStrengthAttempt), typeof(uint)),
                (typeof(MedicineBallPushAttempt), typeof(uint)),
                (typeof(StandingLongJumpAttempt), typeof(uint)),
                (typeof(ShuttleRunAttempt), typeof(uint)),
                (typeof(TwelveMinutesRunAttempt), typeof(float)),
                (typeof(OneLegStandAttempt), typeof(uint))
        };
        foreach (var (type, resultType) in disciplineTypePairs)
        {
            var method = typeof(BestAttemptService).GetMethod(nameof(CalculateBestAttemptAsync))?.MakeGenericMethod(type, resultType);
            if (method != null)
            {
                var task = (Task<BestAttempt?>)method.Invoke(this, [userId, cohortId])!;
                var bestAttempt = await task;
                if (bestAttempt != null)
                    bestAttempts.Add(bestAttempt);
            }
        }
        return bestAttempts;
    }
}
