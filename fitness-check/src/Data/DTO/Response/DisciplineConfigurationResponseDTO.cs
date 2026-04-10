namespace FitnessCheck.Data.DTO.Response;

/// <summary>
/// Response DTO for discipline configuration including data recorder settings.
/// </summary>
public class DisciplineConfigurationResponseDTO
{
    /// <summary>
    /// The discipline name (e.g., "CoreStrength", "MedicineBallPush", etc.).
    /// </summary>
    public required string Discipline { get; set; }
    
    /// <summary>
    /// Data recorder configuration for the discipline.
    /// </summary>
    public required DataRecorderConfigResponseDTO DataRecorderConfig { get; set; }
}