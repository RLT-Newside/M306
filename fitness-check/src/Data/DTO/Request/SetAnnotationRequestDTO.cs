namespace FitnessCheck.Data.DTO.Request;

/// <summary>
/// Request body for annotating a missing fitness-test result.
/// </summary>
public class SetAnnotationRequestDTO
{
    /// <summary>Short annotation text, e.g. "verletzt" or "dispensiert".</summary>
    public required string Annotation { get; set; }
}
