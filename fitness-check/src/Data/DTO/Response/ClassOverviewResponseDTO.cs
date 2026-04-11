namespace FitnessCheck.Data.DTO.Response;

/// <summary>
/// Full class overview returned for the physicalEducationTeacher role.
/// </summary>
public class ClassOverviewResponseDTO
{
    public required CohortResponseDTO Cohort { get; set; }

    public required IEnumerable<ClassOverviewStudentResponseDTO> Students { get; set; }

    /// <summary>Per-discipline averages across the whole class.</summary>
    public required Dictionary<string, ClassDisciplineAverageResponseDTO> ClassAverages { get; set; }
}
