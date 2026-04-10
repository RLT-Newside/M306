using Microsoft.EntityFrameworkCore;

namespace FitnessCheck.Data.Entities;

[Index(nameof(UserId), nameof(CohortId), nameof(Discipline), IsUnique = true)]
public class BestAttempt
{
    /// <summary>
    /// Primary key for the BestAttempt entity.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The ID of the user for whom this best attempt is recorded.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// The ID of the cohort to which the user belongs.
    /// </summary>
    public Guid CohortId { get; set; }

    /// <summary>
    /// The discipline name (e.g. "ShuttleRun", "OneLegStand", etc.).
    /// </summary>
    public required string Discipline { get; set; }

    /// <summary>
    /// The best result value for the discipline (e.g. time, distance, etc.).
    /// </summary>
    public float Result { get; set; }

    /// <summary>
    /// Points awarded for the best result in the discipline.
    /// </summary>
    public uint Points { get; set; }

    /// <summary>
    /// The UTC date/time of the underlying attempt this BestAttempt represents.
    /// </summary>
    public DateTime MomentUtc { get; set; }

    /// <summary>
    /// For OneLegStand discipline: best result for the left foot attempt (if applicable).
    /// </summary>
    public uint? LeftFootResult { get; set; } = null;

    /// <summary>
    /// For OneLegStand discipline: best result for the right foot attempt (if applicable).
    /// </summary>
    public uint? RightFootResult { get; set; } = null;

    /// <summary>
    /// Timestamp when the best attempt was last calculated or updated.
    /// </summary>
    public DateTime LastCalculated { get; set; }
}