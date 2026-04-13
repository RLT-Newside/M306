namespace FitnessCheck.Data.DTO.Response;

/// <summary>
/// DTO representing a single leaderboard entry for a discipline and gender.
/// </summary>
public class LeaderboardEntryResponseDTO
{
    /// <summary>
    /// The best result value (unit depends on discipline).
    /// </summary>
    public required float Result { get; set; }

    /// <summary>
    /// The UTC date/time of the best attempt.
    /// </summary>
    public required DateTime MomentUtc { get; set; }

    /// <summary>
    /// The school year in which the best attempt was recorded (e.g. 2025 for 2025/26).
    /// </summary>
    public required ushort SchoolYear { get; set; }

    /// <summary>
    /// The profession of the student who achieved the best result.
    /// </summary>
    public required string Profession { get; set; }

    /// <summary>
    /// The class name (vocational education) of the student who achieved the best result.
    /// </summary>
    public required string ClassName { get; set; }
}
