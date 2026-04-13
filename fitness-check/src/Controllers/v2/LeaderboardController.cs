using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessCheck.Data;
using FitnessCheck.Data.Entities;
using FitnessCheck.Data.DTO.Response;
using FitnessCheck.Utils;
using Microsoft.AspNetCore.Authorization;

namespace FitnessCheck.Controllers.v2;

/// <summary>
/// API controller for the global gender-specific leaderboard across all fitness test disciplines.
/// For each discipline, returns the best individual attempt (absolute value) per gender,
/// including the date, school year, and profession/class of the record holder.
/// </summary>
[ApiController]
[Route("v2/[controller]")]
public class LeaderboardController(FitnessCheckDbContext _dbContext) : ControllerBase
{
    /// <summary>
    /// Returns the global leaderboard for all disciplines, split by gender.
    /// Each entry contains the best result (absolute value), date, school year, and class/profession.
    /// For OneLegStand the best pre-calculated result (sum of both feet) from BestAttempts is used.
    /// </summary>
    /// <returns>The global leaderboard grouped by discipline.</returns>
    /// <response code="200">Returns the leaderboard.</response>
    /// <response code="401">If the user is not authenticated.</response>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(LeaderboardResponseDTO), 200)]
    [ProducesResponseType(401)]
    [Produces("application/json")]
    public async Task<ActionResult<LeaderboardResponseDTO>> GetLeaderboard()
    {
        var disciplines = new List<LeaderboardDisciplineResponseDTO>
        {
            await BuildAttemptLeaderboard<CoreStrengthAttempt>(
                "CoreStrength",
                a => (float)a.ResultInSeconds,
                lowerIsBetter: false),

            await BuildAttemptLeaderboard<MedicineBallPushAttempt>(
                "MedicineBallPush",
                a => (float)a.ResultInCentimeters,
                lowerIsBetter: false),

            await BuildAttemptLeaderboard<StandingLongJumpAttempt>(
                "StandingLongJump",
                a => (float)a.ResultInCentimeters,
                lowerIsBetter: false),

            // ShuttleRun: lower time (ms) is better
            await BuildAttemptLeaderboard<ShuttleRunAttempt>(
                "ShuttleRun",
                a => (float)a.ResultInMilliseconds,
                lowerIsBetter: true),

            await BuildAttemptLeaderboard<TwelveMinutesRunAttempt>(
                "TwelveMinutesRun",
                a => a.ResultInRounds,
                lowerIsBetter: false),

            // OneLegStand uses BestAttempts because the meaningful result
            // is the sum of the best left-foot and best right-foot attempt.
            await BuildOneLegStandLeaderboard()
        };

        return Ok(new LeaderboardResponseDTO { Disciplines = disciplines });
    }

    /// <summary>
    /// Returns the top N results for a specific discipline and gender, with student names.
    /// </summary>
    /// <param name="discipline">Discipline name (e.g. TwelveMinutesRun).</param>
    /// <param name="gender">Gender filter: 'm' or 'f'.</param>
    /// <param name="limit">Number of entries to return (1–100, default 10).</param>
    [HttpGet("ranking")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<RankedLeaderboardEntryResponseDTO>), 200)]
    [ProducesResponseType(typeof(string), 400)]
    [ProducesResponseType(401)]
    [Produces("application/json")]
    public async Task<ActionResult<IEnumerable<RankedLeaderboardEntryResponseDTO>>> GetRanking(
        [FromQuery] string discipline,
        [FromQuery] char gender = 'm',
        [FromQuery] int limit = 10)
    {
        if (limit < 1 || limit > 100) limit = 10;

        List<(Guid UserId, Guid CohortId, float Result, DateTime MomentUtc)> top;

        top = discipline switch
        {
            "CoreStrength" => await GetTopPerUser<CoreStrengthAttempt>(
                gender, a => (float)a.ResultInSeconds, lowerIsBetter: false, limit),
            "MedicineBallPush" => await GetTopPerUser<MedicineBallPushAttempt>(
                gender, a => (float)a.ResultInCentimeters, lowerIsBetter: false, limit),
            "StandingLongJump" => await GetTopPerUser<StandingLongJumpAttempt>(
                gender, a => (float)a.ResultInCentimeters, lowerIsBetter: false, limit),
            "ShuttleRun" => await GetTopPerUser<ShuttleRunAttempt>(
                gender, a => (float)a.ResultInMilliseconds, lowerIsBetter: true, limit),
            "TwelveMinutesRun" => await GetTopPerUser<TwelveMinutesRunAttempt>(
                gender, a => a.ResultInRounds, lowerIsBetter: false, limit),
            "OneLegStand" => await GetTopOneLegStand(gender, limit),
            _ => null!
        };

        if (top is null)
            return BadRequest($"Unknown discipline '{discipline}'.");

        // Resolve cohorts
        var cohortIds = top.Select(t => t.CohortId).Distinct().ToList();
        var cohorts = await _dbContext.Cohorts
            .Where(c => cohortIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id);

        // Resolve student names
        var userIds = top.Select(t => t.UserId).Distinct().ToList();
        var names = await _dbContext.StudentNames
            .Where(sn => userIds.Contains(sn.UserId))
            .ToDictionaryAsync(sn => sn.UserId);

        var result = top.Select((entry, index) =>
        {
            var displayName = names.TryGetValue(entry.UserId, out var sn)
                ? $"{sn.FirstName} {sn.LastName}".Trim()
                : entry.UserId.ToString()[..8] + "…";

            var className = cohorts.TryGetValue(entry.CohortId, out var cohort)
                ? cohort.ClassNameVocationalEducation
                : "–";

            return new RankedLeaderboardEntryResponseDTO
            {
                Rank = index + 1,
                Name = displayName,
                Result = entry.Result,
                MomentUtc = entry.MomentUtc,
                SchoolYear = SchoolYearUtils.GetSchoolYearForDate(entry.MomentUtc),
                ClassName = className
            };
        }).ToList();

        return Ok(result);
    }

    // ── Helpers ─────────────────────────────────────────────────

    private async Task<List<(Guid UserId, Guid CohortId, float Result, DateTime MomentUtc)>> GetTopPerUser<TAttempt>(
        char gender, Func<TAttempt, float> resultSelector, bool lowerIsBetter, int limit)
        where TAttempt : DisciplineAttempt
    {
        var attempts = await _dbContext.Set<TAttempt>()
            .Where(a => a.Gender == gender)
            .ToListAsync();

        var bestPerUser = attempts
            .GroupBy(a => a.UserId)
            .Select(g => lowerIsBetter
                ? g.MinBy(resultSelector)!
                : g.MaxBy(resultSelector)!)
            .ToList();

        var sorted = lowerIsBetter
            ? bestPerUser.OrderBy(resultSelector)
            : (IEnumerable<TAttempt>)bestPerUser.OrderByDescending(resultSelector);

        return sorted
            .Take(limit)
            .Select(a => (a.UserId, a.CohortId, resultSelector(a), a.MomentUtc))
            .ToList();
    }

    private async Task<List<(Guid UserId, Guid CohortId, float Result, DateTime MomentUtc)>> GetTopOneLegStand(
        char gender, int limit)
    {
        var bestAttempts = await _dbContext.BestAttempts
            .Where(ba => ba.Discipline == "OneLegStand")
            .ToListAsync();

        var userIds = bestAttempts.Select(ba => ba.UserId).Distinct().ToList();
        var genderMap = await _dbContext.OneLegStandAttempts
            .Where(a => userIds.Contains(a.UserId))
            .GroupBy(a => a.UserId)
            .Select(g => new { UserId = g.Key, Gender = g.First().Gender })
            .ToDictionaryAsync(x => x.UserId, x => x.Gender);

        return bestAttempts
            .Where(ba => genderMap.TryGetValue(ba.UserId, out var g) && g == gender)
            .OrderByDescending(ba => ba.Result)
            .Take(limit)
            .Select(ba => (ba.UserId, ba.CohortId, ba.Result, ba.MomentUtc))
            .ToList();
    }

    private async Task<LeaderboardDisciplineResponseDTO> BuildAttemptLeaderboard<TAttempt>(
        string disciplineName,
        Func<TAttempt, float> resultSelector,
        bool lowerIsBetter)
        where TAttempt : DisciplineAttempt
    {
        var attempts = await _dbContext.Set<TAttempt>()
            .Include(a => a.Cohort)
            .ToListAsync();

        return new LeaderboardDisciplineResponseDTO
        {
            Discipline = disciplineName,
            Male = GetBestEntry(attempts, 'm', resultSelector, lowerIsBetter),
            Female = GetBestEntry(attempts, 'f', resultSelector, lowerIsBetter)
        };
    }

    /// <summary>
    /// Finds the best attempt entry for a given gender from a list of attempts.
    /// </summary>
    private static LeaderboardEntryResponseDTO? GetBestEntry<TAttempt>(
        IEnumerable<TAttempt> attempts,
        char gender,
        Func<TAttempt, float> resultSelector,
        bool lowerIsBetter)
        where TAttempt : DisciplineAttempt
    {
        var filtered = attempts.Where(a => a.Gender == gender);
        var best = lowerIsBetter
            ? filtered.MinBy(resultSelector)
            : filtered.MaxBy(resultSelector);

        if (best == null) return null;

        return new LeaderboardEntryResponseDTO
        {
            Result = resultSelector(best),
            MomentUtc = best.MomentUtc,
            SchoolYear = SchoolYearUtils.GetSchoolYearForDate(best.MomentUtc),
            Profession = best.Cohort.Profession,
            ClassName = best.Cohort.ClassNameVocationalEducation
        };
    }

    /// <summary>
    /// Builds the OneLegStand leaderboard entry using BestAttempts (which pre-calculate
    /// the combined left + right foot result). Gender is resolved from OneLegStandAttempts.
    /// </summary>
    private async Task<LeaderboardDisciplineResponseDTO> BuildOneLegStandLeaderboard()
    {
        var bestAttempts = await _dbContext.BestAttempts
            .Where(ba => ba.Discipline == "OneLegStand")
            .ToListAsync();

        if (bestAttempts.Count == 0)
        {
            return new LeaderboardDisciplineResponseDTO
            {
                Discipline = "OneLegStand",
                Male = null,
                Female = null
            };
        }

        // Resolve gender per user from the OneLegStandAttempts table
        var userIds = bestAttempts.Select(ba => ba.UserId).Distinct().ToList();
        var userGenders = await _dbContext.OneLegStandAttempts
            .Where(a => userIds.Contains(a.UserId))
            .GroupBy(a => a.UserId)
            .Select(g => new { UserId = g.Key, Gender = g.First().Gender })
            .ToDictionaryAsync(x => x.UserId, x => x.Gender);

        // Resolve cohort info
        var cohortIds = bestAttempts.Select(ba => ba.CohortId).Distinct().ToList();
        var cohorts = await _dbContext.Cohorts
            .Where(c => cohortIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id);

        // Build enriched entries with gender and cohort info
        var entries = bestAttempts
            .Where(ba => userGenders.ContainsKey(ba.UserId) && cohorts.ContainsKey(ba.CohortId))
            .Select(ba => new
            {
                ba.Result,
                ba.MomentUtc,
                Gender = userGenders[ba.UserId],
                Cohort = cohorts[ba.CohortId]
            })
            .ToList();

        LeaderboardEntryResponseDTO? BuildEntry(char gender) =>
            entries
                .Where(e => e.Gender == gender)
                .MaxBy(e => e.Result) is { } best
                ? new LeaderboardEntryResponseDTO
                {
                    Result = best.Result,
                    MomentUtc = best.MomentUtc,
                    SchoolYear = SchoolYearUtils.GetSchoolYearForDate(best.MomentUtc),
                    Profession = best.Cohort.Profession,
                    ClassName = best.Cohort.ClassNameVocationalEducation
                }
                : null;

        return new LeaderboardDisciplineResponseDTO
        {
            Discipline = "OneLegStand",
            Male = BuildEntry('m'),
            Female = BuildEntry('f')
        };
    }
}
