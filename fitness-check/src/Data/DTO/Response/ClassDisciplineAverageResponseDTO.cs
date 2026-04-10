namespace FitnessCheck.Data.DTO.Response;

/// <summary>
/// Class average metrics for one discipline.
/// </summary>
public class ClassDisciplineAverageResponseDTO
{
    /// <summary>Average of best results across all students who have a result.</summary>
    public float? AverageResult { get; set; }

    /// <summary>Average of points across all students who have a result.</summary>
    public float? AveragePoints { get; set; }
}
