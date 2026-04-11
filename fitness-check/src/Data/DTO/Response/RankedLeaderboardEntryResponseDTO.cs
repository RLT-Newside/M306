namespace FitnessCheck.Data.DTO.Response;

/// <summary>
/// One entry in a ranked discipline leaderboard, including student name and class.
/// </summary>
public class RankedLeaderboardEntryResponseDTO
{
    public int Rank { get; set; }
    public required string Name { get; set; }
    public float Result { get; set; }
    public DateTime MomentUtc { get; set; }
    public ushort SchoolYear { get; set; }
    public required string ClassName { get; set; }
}
