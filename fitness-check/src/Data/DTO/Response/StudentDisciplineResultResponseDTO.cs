namespace FitnessCheck.Data.DTO.Response;

/// <summary>
/// The best result for one student in one discipline.
/// </summary>
public class StudentDisciplineResultResponseDTO
{
    /// <summary>Best result value (unit depends on the discipline), or null if no result exists.</summary>
    public float? Result { get; set; }

    /// <summary>Points awarded for the best result (0 when no result).</summary>
    public uint Points { get; set; }

    /// <summary>UTC timestamp of the best attempt, or null if no result exists.</summary>
    public DateTime? MomentUtc { get; set; }

    /// <summary>
    /// Teacher annotation for a missing result (e.g. "verletzt", "dispensiert"), or null.
    /// </summary>
    public string? Annotation { get; set; }
}
