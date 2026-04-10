namespace FitnessCheck.Data.DTO.Request;

/// <summary>
/// Request body for a teacher setting or correcting a student's best result.
/// </summary>
public class SetResultRequestDTO
{
    /// <summary>
    /// The new result value in the discipline's native unit
    /// (seconds, centimetres, milliseconds, rounds, etc.).
    /// </summary>
    public required float Result { get; set; }

    /// <summary>Student gender needed to recalculate points ('m' or 'f').</summary>
    public required char Gender { get; set; }
}
