using System.ComponentModel.DataAnnotations;

namespace FitnessCheck.Data.DTO.Request;

public class StandingLongJumpAttemptCreationRequestDTO
{
    /// <summary>
    /// The distance for the standing long jump attempt in centimeters.
    /// </summary>
    [Range(50, 350, ErrorMessage = "Result in centimeters must be in range 50 - 350.")]
    [Required(ErrorMessage = "The result in centimeters must be provided.")]
    public required uint ResultInCentimeters { get; set; }
}
