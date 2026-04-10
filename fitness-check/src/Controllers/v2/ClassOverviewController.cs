using AutoMapper;
using FitnessCheck.Data;
using FitnessCheck.Data.DTO.Request;
using FitnessCheck.Data.DTO.Response;
using FitnessCheck.Data.Entities;
using FitnessCheck.Services;
using FitnessCheck.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessCheck.Controllers.v2;

/// <summary>
/// API controller for the sports-teacher class overview.
/// Provides read and write access to all students' fitness-test results within a cohort.
/// All endpoints require the <c>physicalEducationTeacher</c> role.
/// </summary>
[ApiController]
[Route("v2/[controller]")]
[Authorize]
public class ClassOverviewController(
    FitnessCheckDbContext _dbContext,
    IMapper _mapper,
    IPointsCalculator _pointsCalculator) : ControllerBase
{
    private static readonly string[] DisciplineNames =
    [
        "CoreStrength", "MedicineBallPush", "StandingLongJump",
        "ShuttleRun", "TwelveMinutesRun", "OneLegStand"
    ];

    // ─────────────────────────────────────────────────────────────
    // GET  /v2/classOverview/cohorts
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns all cohorts that have at least one recorded best attempt.
    /// </summary>
    [HttpGet("cohorts")]
    [ProducesResponseType(typeof(IEnumerable<CohortResponseDTO>), 200)]
    [ProducesResponseType(401)]
    [Produces("application/json")]
    public async Task<ActionResult<IEnumerable<CohortResponseDTO>>> GetCohorts()
    {
        // Only return cohorts that actually have student data.
        var cohortIds = await _dbContext.BestAttempts
            .Select(ba => ba.CohortId)
            .Distinct()
            .ToListAsync();

        var cohorts = await _dbContext.Cohorts
            .Where(c => cohortIds.Contains(c.Id))
            .OrderBy(c => c.ClassNameVocationalEducation)
            .ToListAsync();

        return Ok(_mapper.Map<IEnumerable<CohortResponseDTO>>(cohorts));
    }

    // ─────────────────────────────────────────────────────────────
    // GET  /v2/classOverview/{cohortId}
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns the full class overview for the given cohort:
    /// all students with best attempts per discipline, class averages, and annotations.
    /// </summary>
    [HttpGet("{cohortId:guid}")]
    [ProducesResponseType(typeof(ClassOverviewResponseDTO), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    [Produces("application/json")]
    public async Task<ActionResult<ClassOverviewResponseDTO>> GetClassOverview(Guid cohortId)
    {
        var cohort = await _dbContext.Cohorts.FindAsync(cohortId);
        if (cohort is null) return NotFound($"Cohort {cohortId} not found.");

        // All BestAttempts for this cohort
        var bestAttempts = await _dbContext.BestAttempts
            .Where(ba => ba.CohortId == cohortId)
            .ToListAsync();

        var userIds = bestAttempts.Select(ba => ba.UserId).Distinct().ToList();

        // Student names (best-effort: may be empty for users who never submitted via the app)
        var studentNames = await _dbContext.StudentNames
            .Where(sn => userIds.Contains(sn.UserId))
            .ToDictionaryAsync(sn => sn.UserId);

        // Annotations for missing results
        var annotations = await _dbContext.MissingResultAnnotations
            .Where(a => a.CohortId == cohortId)
            .ToListAsync();

        // Gender – resolve from any discipline attempt
        var genderByUser = await ResolveGendersAsync(userIds);

        // Build per-student DTOs
        var students = new List<ClassOverviewStudentResponseDTO>();
        foreach (var userId in userIds)
        {
            var userAttempts = bestAttempts.Where(ba => ba.UserId == userId).ToList();
            var name = studentNames.TryGetValue(userId, out var sn) ? sn : null;
            var gender = genderByUser.TryGetValue(userId, out var g) ? g : '?';

            var disciplines = BuildDisciplineResults(userId, userAttempts, annotations);

            var attemptPoints = userAttempts.Select(ba => (int)ba.Points).ToList();
            uint? totalPoints = attemptPoints.Count > 0 ? (uint)attemptPoints.Sum() : null;
            float? avgPoints = attemptPoints.Count > 0 ? (float)attemptPoints.Average() : null;

            students.Add(new ClassOverviewStudentResponseDTO
            {
                UserId = userId,
                FirstName = name?.FirstName ?? string.Empty,
                LastName = name?.LastName ?? string.Empty,
                Gender = gender,
                Disciplines = disciplines,
                TotalPoints = totalPoints,
                AveragePoints = avgPoints,
                Rating = avgPoints.HasValue ? GradingUtils.GetRating((uint)avgPoints.Value) : null
            });
        }

        // Sort students by last name, then first name
        students = [.. students.OrderBy(s => s.LastName).ThenBy(s => s.FirstName)];

        // Class averages per discipline
        var classAverages = BuildClassAverages(bestAttempts);

        return Ok(new ClassOverviewResponseDTO
        {
            Cohort = _mapper.Map<CohortResponseDTO>(cohort),
            Students = students,
            ClassAverages = classAverages
        });
    }

    // ─────────────────────────────────────────────────────────────
    // PUT  /v2/classOverview/{cohortId}/students/{userId}/disciplines/{discipline}
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Sets (creates or updates) the best result for a specific student and discipline.
    /// Points are recalculated automatically. Any existing annotation is removed.
    /// </summary>
    [HttpPut("{cohortId:guid}/students/{userId:guid}/disciplines/{discipline}")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(StudentDisciplineResultResponseDTO), 200)]
    [ProducesResponseType(typeof(string), 400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    [Produces("application/json")]
    public async Task<ActionResult<StudentDisciplineResultResponseDTO>> SetResult(
        Guid cohortId, Guid userId, string discipline, [FromBody] SetResultRequestDTO request)
    {
        if (!DisciplineNames.Contains(discipline))
            return BadRequest($"Unknown discipline '{discipline}'.");

        var cohort = await _dbContext.Cohorts.FindAsync(cohortId);
        if (cohort is null) return NotFound($"Cohort {cohortId} not found.");

        // Calculate points for the new result
        var points = CalculatePoints(discipline, request.Result, request.Gender);

        var best = await _dbContext.BestAttempts.FirstOrDefaultAsync(
            ba => ba.UserId == userId && ba.CohortId == cohortId && ba.Discipline == discipline);

        if (best is null)
        {
            best = new BestAttempt
            {
                UserId = userId,
                CohortId = cohortId,
                Discipline = discipline
            };
            _dbContext.BestAttempts.Add(best);
        }

        best.Result = request.Result;
        best.Points = points;
        best.MomentUtc = DateTime.UtcNow;

        // For OneLegStand, clear foot-specific fields as the teacher sets the combined value directly
        if (discipline == "OneLegStand")
        {
            best.LeftFootResult = null;
            best.RightFootResult = null;
        }

        // Remove any annotation now that a result exists
        var annotation = await _dbContext.MissingResultAnnotations.FirstOrDefaultAsync(
            a => a.UserId == userId && a.CohortId == cohortId && a.Discipline == discipline);
        if (annotation is not null)
            _dbContext.MissingResultAnnotations.Remove(annotation);

        await _dbContext.SaveChangesAsync();

        return Ok(new StudentDisciplineResultResponseDTO
        {
            Result = best.Result,
            Points = best.Points,
            MomentUtc = best.MomentUtc,
            Annotation = null
        });
    }

    // ─────────────────────────────────────────────────────────────
    // DELETE  /v2/classOverview/{cohortId}/students/{userId}/disciplines/{discipline}
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Removes the best result for a specific student and discipline.
    /// </summary>
    [HttpDelete("{cohortId:guid}/students/{userId:guid}/disciplines/{discipline}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteResult(Guid cohortId, Guid userId, string discipline)
    {
        var best = await _dbContext.BestAttempts.FirstOrDefaultAsync(
            ba => ba.UserId == userId && ba.CohortId == cohortId && ba.Discipline == discipline);

        if (best is null) return NotFound();

        _dbContext.BestAttempts.Remove(best);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    // ─────────────────────────────────────────────────────────────
    // PUT  /v2/classOverview/{cohortId}/students/{userId}/disciplines/{discipline}/annotation
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Sets or updates a teacher annotation for a missing result
    /// (e.g. "verletzt", "dispensiert"). Passing an empty string removes the annotation.
    /// </summary>
    [HttpPut("{cohortId:guid}/students/{userId:guid}/disciplines/{discipline}/annotation")]
    [Consumes("application/json")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(string), 400)]
    [ProducesResponseType(401)]
    [Produces("application/json")]
    public async Task<IActionResult> SetAnnotation(
        Guid cohortId, Guid userId, string discipline, [FromBody] SetAnnotationRequestDTO request)
    {
        if (!DisciplineNames.Contains(discipline))
            return BadRequest($"Unknown discipline '{discipline}'.");

        if (string.IsNullOrWhiteSpace(request.Annotation))
        {
            // Remove annotation
            var toRemove = await _dbContext.MissingResultAnnotations.FirstOrDefaultAsync(
                a => a.UserId == userId && a.CohortId == cohortId && a.Discipline == discipline);
            if (toRemove is not null)
            {
                _dbContext.MissingResultAnnotations.Remove(toRemove);
                await _dbContext.SaveChangesAsync();
            }
            return Ok();
        }

        var annotation = await _dbContext.MissingResultAnnotations.FirstOrDefaultAsync(
            a => a.UserId == userId && a.CohortId == cohortId && a.Discipline == discipline);

        if (annotation is null)
        {
            _dbContext.MissingResultAnnotations.Add(new MissingResultAnnotation
            {
                UserId = userId,
                CohortId = cohortId,
                Discipline = discipline,
                Annotation = request.Annotation
            });
        }
        else
        {
            annotation.Annotation = request.Annotation;
        }

        await _dbContext.SaveChangesAsync();
        return Ok();
    }

    // ─────────────────────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────────────────────

    /// <summary>Resolves the gender for each user from any discipline attempt table.</summary>
    private async Task<Dictionary<Guid, char>> ResolveGendersAsync(List<Guid> userIds)
    {
        // Collect (UserId, Gender) pairs from all discipline attempt tables.
        var pairs = new List<(Guid UserId, char Gender)>();

        async Task Collect<T>() where T : DisciplineAttempt
        {
            var rows = await _dbContext.Set<T>()
                .Where(a => userIds.Contains(a.UserId))
                .GroupBy(a => a.UserId)
                .Select(g => new { UserId = g.Key, Gender = g.First().Gender })
                .ToListAsync();
            pairs.AddRange(rows.Select(r => (r.UserId, r.Gender)));
        }

        await Collect<CoreStrengthAttempt>();
        await Collect<MedicineBallPushAttempt>();
        await Collect<StandingLongJumpAttempt>();
        await Collect<ShuttleRunAttempt>();
        await Collect<TwelveMinutesRunAttempt>();
        await Collect<OneLegStandAttempt>();

        // Take the first gender found for each user
        return pairs
            .GroupBy(p => p.UserId)
            .ToDictionary(g => g.Key, g => g.First().Gender);
    }

    private static Dictionary<string, StudentDisciplineResultResponseDTO> BuildDisciplineResults(
        Guid userId,
        List<BestAttempt> userAttempts,
        List<MissingResultAnnotation> annotations)
    {
        var result = new Dictionary<string, StudentDisciplineResultResponseDTO>();
        foreach (var discipline in DisciplineNames)
        {
            var attempt = userAttempts.FirstOrDefault(ba => ba.Discipline == discipline);
            var annotation = annotations
                .FirstOrDefault(a => a.UserId == userId && a.Discipline == discipline);

            result[discipline] = new StudentDisciplineResultResponseDTO
            {
                Result = attempt?.Result,
                Points = attempt?.Points ?? 0,
                MomentUtc = attempt?.MomentUtc,
                Annotation = annotation?.Annotation
            };
        }
        return result;
    }

    private static Dictionary<string, ClassDisciplineAverageResponseDTO> BuildClassAverages(
        List<BestAttempt> bestAttempts)
    {
        var result = new Dictionary<string, ClassDisciplineAverageResponseDTO>();
        foreach (var discipline in DisciplineNames)
        {
            var disciplineAttempts = bestAttempts.Where(ba => ba.Discipline == discipline).ToList();
            result[discipline] = new ClassDisciplineAverageResponseDTO
            {
                AverageResult = disciplineAttempts.Count > 0
                    ? disciplineAttempts.Average(ba => ba.Result)
                    : null,
                AveragePoints = disciplineAttempts.Count > 0
                    ? (float)disciplineAttempts.Average(ba => (double)ba.Points)
                    : null
            };
        }
        return result;
    }

    private uint CalculatePoints(string discipline, float result, char gender)
    {
        Func<FitnessCheckModels.ResultsCalculation, bool> filter = discipline switch
        {
            "CoreStrength" => rc => rc.CoreStrength <= (int)result && rc.Gender == gender,
            "MedicineBallPush" => rc => rc.MedicineBallPush <= (int)result && rc.Gender == gender,
            "StandingLongJump" => rc => rc.StandingLongJump <= (int)result && rc.Gender == gender,
            "ShuttleRun" => rc => rc.ShuttleRun >= (int)result && rc.Gender == gender,
            "TwelveMinutesRun" => rc => rc.TwelveMinutesRun <= result && rc.Gender == gender,
            "OneLegStand" => rc => rc.OneLegStand <= (int)result && rc.Gender == gender,
            _ => _ => false
        };
        return _pointsCalculator.CalculatePoints(filter);
    }
}
