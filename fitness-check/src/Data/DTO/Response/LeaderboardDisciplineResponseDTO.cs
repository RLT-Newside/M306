namespace FitnessCheck.Data.DTO.Response;

/// <summary>
/// DTO representing the global leaderboard entry for a single discipline, split by gender.
/// </summary>
public class LeaderboardDisciplineResponseDTO
{
    /// <summary>
    /// The discipline name (e.g. "CoreStrength", "ShuttleRun").
    /// </summary>
    public required string Discipline { get; set; }

    /// <summary>
    /// The best male result for this discipline, or null if no data exists.
    /// </summary>
    public LeaderboardEntryResponseDTO? Male { get; set; }

    /// <summary>
    /// The best female result for this discipline, or null if no data exists.
    /// </summary>
    public LeaderboardEntryResponseDTO? Female { get; set; }
}
