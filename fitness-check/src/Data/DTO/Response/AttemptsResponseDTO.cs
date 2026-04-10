using System.Numerics;

namespace FitnessCheck.Data.DTO.Response;

/// <summary>
/// Response DTO containing attempts data and related information for a specific discipline.
/// </summary>
/// <typeparam name="TDiscipline">The specific discipline attempt response DTO type.</typeparam>
/// <typeparam name="TResultValue">The result value type for the discipline.</typeparam>
public class AttemptsResponseDTO<TDiscipline, TResultValue>
    where TDiscipline : DisciplineAttemptResponseDTO
    where TResultValue : INumber<TResultValue>
{
    /// <summary>
    /// The list of attempts made for this discipline in the current school year.
    /// </summary>
    public IEnumerable<TDiscipline> Attempts { get; set; } = [];
    
    /// <summary>
    /// The number of attempts remaining that can be made for this discipline.
    /// </summary>
    public uint RemainingAttempts { get; set; }
    
    /// <summary>
    /// The maximum number of attempts allowed for this discipline.
    /// For OneLegStand, this represents the total attempts across both feet.
    /// </summary>
    public uint MaxAllowedAttempts { get; set; }
    
    /// <summary>
    /// The points achieved from the best attempt for this discipline.
    /// </summary>
    public uint Points { get; set; }
    
    /// <summary>
    /// The average result of the cohort for this discipline, if available.
    /// </summary>
    public float? CohortAverageResult { get; set; }
    
    /// <summary>
    /// The average points of the cohort for this discipline, if available.
    /// </summary>
    public float? CohortAveragePoints { get; set; }
    
    /// <summary>
    /// Annual performance data showing progression over multiple school years.
    /// </summary>
    public IEnumerable<AnnualPerformanceResponseDTO> AnnualPerformance { get; set; } = [];
    
    /// <summary>
    /// The school year for which these attempts are retrieved.
    /// </summary>
    public ushort SchoolYear { get; set; }
    
    /// <summary>
    /// Indicates whether the retrieved school year is the current school year.
    /// </summary>
    public bool IsCurrentSchoolYear { get; set; }
    
    /// <summary>
    /// The gender of the user ('m' for male, 'f' for female).
    /// Used for gender-specific result ranges and evaluation criteria.
    /// </summary>
    public char Gender { get; set; }
}