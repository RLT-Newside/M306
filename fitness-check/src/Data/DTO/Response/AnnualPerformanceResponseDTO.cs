namespace FitnessCheck.Data.DTO.Response;

public class AnnualPerformanceResponseDTO
{
    public ushort Year { get; set; }
    public float? Result { get; set; }
    public uint? Points { get; set; }
}