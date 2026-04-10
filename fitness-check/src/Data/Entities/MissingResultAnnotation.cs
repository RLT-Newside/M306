using Microsoft.EntityFrameworkCore;

namespace FitnessCheck.Data.Entities;

/// <summary>
/// Records a teacher annotation for a missing fitness test result
/// (e.g. "verletzt" or "dispensiert").
/// Each combination of (UserId, CohortId, Discipline) is unique.
/// </summary>
[Index(nameof(UserId), nameof(CohortId), nameof(Discipline), IsUnique = true)]
public class MissingResultAnnotation
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public Guid CohortId { get; set; }

    /// <summary>Discipline name, e.g. "CoreStrength".</summary>
    public required string Discipline { get; set; }

    /// <summary>Short annotation text, e.g. "verletzt" or "dispensiert".</summary>
    public required string Annotation { get; set; }
}
