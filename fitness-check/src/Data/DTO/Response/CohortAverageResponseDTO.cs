using System.Numerics;

namespace FitnessCheck.Data.DTO.Response;

public class CohortAverageResponseDTO<TResultValue> where TResultValue : INumber<TResultValue>
{
    public float? AverageResult { get; set; }
    public float? AveragePoints { get; set; }
}