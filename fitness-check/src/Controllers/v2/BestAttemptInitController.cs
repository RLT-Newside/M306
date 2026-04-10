using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessCheck.Data;
using FitnessCheck.Data.Entities;
using FitnessCheck.Services;

namespace FitnessCheck.Controllers.v2;

[ApiController]
[Route("v2/[controller]")]
public class BestAttemptInitController(FitnessCheckDbContext _dbContext, IPointsCalculator _pointsCalculator) : ControllerBase
{
    [HttpPost("generate-initial-best-attempts")]
    public async Task<IActionResult> GenerateInitialBestAttempts()
    {
        // CORE STRENGTH
        var coreStrengthGroups = await _dbContext.Set<CoreStrengthAttempt>()
            .GroupBy(a => new { a.UserId, a.CohortId })
            .ToListAsync();

        foreach (var group in coreStrengthGroups)
        {
            var best = group.OrderByDescending(a => a.ResultInSeconds).FirstOrDefault(); // Higher is better
            if (best != null)
            {
                var exists = await _dbContext.BestAttempts.AnyAsync(b => b.UserId == best.UserId && b.CohortId == best.CohortId && b.Discipline == "CoreStrength");
                if (!exists)
                {
                    var pointsFilter = best.GetPointsFilter(best.ResultInSeconds, best.Gender);
                    var points = _pointsCalculator.CalculatePoints(pointsFilter);
                    var bestAttempt = new BestAttempt
                    {
                        UserId = best.UserId,
                        CohortId = best.CohortId,
                        Discipline = "CoreStrength",
                        Result = best.ResultInSeconds,
                        Points = points,
                        MomentUtc = best.MomentUtc,
                        LastCalculated = DateTime.UtcNow
                    };
                    _dbContext.BestAttempts.Add(bestAttempt);
                }
            }
        }

        // MEDICINE BALL PUSH
        var medicineBallPushGroups = await _dbContext.Set<MedicineBallPushAttempt>()
            .GroupBy(a => new { a.UserId, a.CohortId })
            .ToListAsync();

        foreach (var group in medicineBallPushGroups)
        {
            var best = group.OrderByDescending(a => a.ResultInCentimeters).FirstOrDefault(); // Higher is better
            if (best != null)
            {
                var exists = await _dbContext.BestAttempts.AnyAsync(b => b.UserId == best.UserId && b.CohortId == best.CohortId && b.Discipline == "MedicineBallPush");
                if (!exists)
                {
                    var pointsFilter = best.GetPointsFilter(best.ResultInCentimeters, best.Gender);
                    var points = _pointsCalculator.CalculatePoints(pointsFilter);
                    var bestAttempt = new BestAttempt
                    {
                        UserId = best.UserId,
                        CohortId = best.CohortId,
                        Discipline = "MedicineBallPush",
                        Result = best.ResultInCentimeters,
                        Points = points,
                        MomentUtc = best.MomentUtc,
                        LastCalculated = DateTime.UtcNow
                    };
                    _dbContext.BestAttempts.Add(bestAttempt);
                }
            }
        }

        // ONE LEG STAND
        var oneLegStandAttempts = await _dbContext.Set<OneLegStandAttempt>()
            .GroupBy(a => new { a.UserId, a.CohortId })
            .ToListAsync();

        foreach (var group in oneLegStandAttempts)
        {
            var bestLeft = group.Where(attempt => attempt.Foot == EFoot.Left).OrderByDescending(a => a.ResultInSeconds).FirstOrDefault(); // Higher is better
            var bestRight = group.Where(attempt => attempt.Foot == EFoot.Right).OrderByDescending(a => a.ResultInSeconds).FirstOrDefault(); // Higher is better
            if (bestLeft != null && bestRight != null)
            {
                var exists = await _dbContext.BestAttempts.AnyAsync(b => b.UserId == bestLeft.UserId && b.CohortId == bestLeft.CohortId && b.Discipline == "OneLegStand");
                if (!exists)
                {
                    var resultValue = bestLeft.ResultInSeconds + bestRight.ResultInSeconds;
                    var pointsFilter = bestLeft.GetPointsFilter(resultValue, bestLeft.Gender);
                    var points = _pointsCalculator.CalculatePoints(pointsFilter);
                    // Use the later of the two attempt moments
                    var attemptMomentUtc = bestLeft.MomentUtc > bestRight.MomentUtc ? bestLeft.MomentUtc : bestRight.MomentUtc;
                    var bestAttempt = new BestAttempt
                    {
                        UserId = bestLeft.UserId,
                        CohortId = bestLeft.CohortId,
                        Discipline = "OneLegStand",
                        Result = resultValue,
                        Points = points,
                        MomentUtc = attemptMomentUtc,
                        LastCalculated = DateTime.UtcNow,
                        LeftFootResult = bestLeft.ResultInSeconds,
                        RightFootResult = bestRight.ResultInSeconds,
                    };
                    _dbContext.BestAttempts.Add(bestAttempt);
                }
            }
        }

        // SHUTTLE RUN
        var shuttleRunGroups = await _dbContext.Set<ShuttleRunAttempt>()
            .GroupBy(a => new { a.UserId, a.CohortId })
            .ToListAsync();

        foreach (var group in shuttleRunGroups)
        {
            var best = group.OrderBy(a => a.ResultInMilliseconds).FirstOrDefault(); // Lower is better
            if (best != null)
            {
                var exists = await _dbContext.BestAttempts.AnyAsync(b => b.UserId == best.UserId && b.CohortId == best.CohortId && b.Discipline == "ShuttleRun");
                if (!exists)
                {
                    var pointsFilter = best.GetPointsFilter(best.ResultInMilliseconds, best.Gender);
                    var points = _pointsCalculator.CalculatePoints(pointsFilter);
                    var bestAttempt = new BestAttempt
                    {
                        UserId = best.UserId,
                        CohortId = best.CohortId,
                        Discipline = "ShuttleRun",
                        Result = best.ResultInMilliseconds,
                        Points = points,
                        MomentUtc = best.MomentUtc,
                        LastCalculated = DateTime.UtcNow
                    };
                    _dbContext.BestAttempts.Add(bestAttempt);
                }
            }
        }

        // STANDING LONG JUMP
        var standingLongJumpGroups = await _dbContext.Set<StandingLongJumpAttempt>()
            .GroupBy(a => new { a.UserId, a.CohortId })
            .ToListAsync();

        foreach (var group in standingLongJumpGroups)
        {
            var best = group.OrderByDescending(a => a.ResultInCentimeters).FirstOrDefault(); // Higher is better
            if (best != null)
            {
                var exists = await _dbContext.BestAttempts.AnyAsync(b => b.UserId == best.UserId && b.CohortId == best.CohortId && b.Discipline == "StandingLongJump");
                if (!exists)
                {
                    var pointsFilter = best.GetPointsFilter(best.ResultInCentimeters, best.Gender);
                    var points = _pointsCalculator.CalculatePoints(pointsFilter);
                    var bestAttempt = new BestAttempt
                    {
                        UserId = best.UserId,
                        CohortId = best.CohortId,
                        Discipline = "StandingLongJump",
                        Result = best.ResultInCentimeters,
                        Points = points,
                        MomentUtc = best.MomentUtc,
                        LastCalculated = DateTime.UtcNow
                    };
                    _dbContext.BestAttempts.Add(bestAttempt);
                }
            }
        }

        // TWELVE MINUTES RUN
        var twelveMinutesRunGroups = await _dbContext.Set<TwelveMinutesRunAttempt>()
            .GroupBy(a => new { a.UserId, a.CohortId })
            .ToListAsync();

        foreach (var group in twelveMinutesRunGroups)
        {
            var best = group.OrderByDescending(a => a.ResultInRounds).FirstOrDefault(); // Higher is better
            if (best != null)
            {
                var exists = await _dbContext.BestAttempts.AnyAsync(b => b.UserId == best.UserId && b.CohortId == best.CohortId && b.Discipline == "TwelveMinutesRun");
                if (!exists)
                {
                    var pointsFilter = best.GetPointsFilter(best.ResultInRounds, best.Gender);
                    var points = _pointsCalculator.CalculatePoints(pointsFilter);
                    var bestAttempt = new BestAttempt
                    {
                        UserId = best.UserId,
                        CohortId = best.CohortId,
                        Discipline = "TwelveMinutesRun",
                        Result = best.ResultInRounds,
                        Points = points,
                        MomentUtc = best.MomentUtc,
                        LastCalculated = DateTime.UtcNow
                    };
                    _dbContext.BestAttempts.Add(bestAttempt);
                }
            }
        }

        await _dbContext.SaveChangesAsync();
        return Ok("Initial BestAttempt entries generated.");
    }
}
