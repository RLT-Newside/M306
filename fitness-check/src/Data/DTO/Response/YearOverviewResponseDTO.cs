namespace FitnessCheck.Data.DTO.Response;

/// <summary>
/// DTO representing a yearly overview of best attempts and cohort information.
/// </summary>
public class YearOverviewResponseDTO
{
    /// <summary>
    /// The school year (e.g., 2025).
    /// </summary>
    public required ushort SchoolYear { get; set; }

    /// <summary>
    /// The best attempts for the user in this school year.
    /// </summary>
    public required IEnumerable<BestAttemptResponseDTO> BestAttempts { get; set; } = [];

    /// <summary>
    /// The total points for the user in this school year.
    /// </summary>
    public uint? TotalPoints { get; set; }

    /// <summary>
    /// The average points for the user in this school year.
    /// </summary>
    public float? AveragePoints { get; set; }

    /// <summary>
    /// The fitness rating for the user in this school year (e.g., "UED", "UEE", etc.).
    /// </summary>
    public string? Rating { get; set; }

    /// <summary>
    /// The cohort information for this school year.
    /// </summary>
    public required CohortResponseDTO Cohort { get; set; }
}