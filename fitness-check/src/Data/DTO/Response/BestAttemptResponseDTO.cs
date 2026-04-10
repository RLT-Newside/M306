namespace FitnessCheck.Data.DTO.Response;

public class BestAttemptResponseDTO
{
    public required Guid UserId { get; set; }
    public required Guid CohortId { get; set; }
    public required string Discipline { get; set; }
    public required float Result { get; set; }
    public required uint Points { get; set; }
    public DateTime MomentUtc { get; set; }
    public uint? LeftFootResult { get; set; }
    public uint? RightFootResult { get; set; }
}