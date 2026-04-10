namespace FitnessCheck.Data.DTO.Response;

/// <summary>
/// DTO representing cohort information for a user.
/// </summary>
public class CohortResponseDTO
{
    /// <summary>
    /// The unique identifier of the cohort.
    /// </summary>
    public required Guid Id { get; set; }

    /// <summary>
    /// The profession of the cohort.
    /// </summary>
    public required string Profession { get; set; }

    /// <summary>
    /// Whether the cohort is a baccalaureate.
    /// </summary>
    public required bool Baccalaureate { get; set; }

    /// <summary>
    /// The index of the school year (e.g., 1-4).
    /// </summary>
    public required byte SchoolYear { get; set; }

    /// <summary>
    /// The first school year of the cohort (e.g., 2023).
    /// </summary>
    public required ushort FirstSchoolYear { get; set; }

    /// <summary>
    /// The class name for vocational education.
    /// </summary>
    public required string ClassNameVocationalEducation { get; set; }

    /// <summary>
    /// The class name for baccalaureate, if applicable.
    /// </summary>
    public required string ClassNameBaccalaureate { get; set; }
}