namespace FitnessCheck.Data.DTO.Response;

/// <summary>
/// DTO representing the global leaderboard across all disciplines.
/// </summary>
public class LeaderboardResponseDTO
{
    /// <summary>
    /// The leaderboard entries per discipline, each containing the best male and female result.
    /// </summary>
    public required IEnumerable<LeaderboardDisciplineResponseDTO> Disciplines { get; set; }
}
