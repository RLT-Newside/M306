namespace FitnessCheck.Data.DTO.Response;

/// <summary>
/// All fitness-test data for one student in a class overview.
/// </summary>
public class ClassOverviewStudentResponseDTO
{
    public required Guid UserId { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    /// <summary>Student gender: 'm' or 'f'.</summary>
    public required char Gender { get; set; }

    /// <summary>Best results per discipline, keyed by discipline name.</summary>
    public required Dictionary<string, StudentDisciplineResultResponseDTO> Disciplines { get; set; }

    /// <summary>Sum of all discipline points (null when no results at all).</summary>
    public uint? TotalPoints { get; set; }

    /// <summary>Average discipline points (null when no results at all).</summary>
    public float? AveragePoints { get; set; }

    /// <summary>Overall rating string (UED/UEE/EE/TE), or null when no results.</summary>
    public string? Rating { get; set; }
}
