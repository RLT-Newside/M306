using System.ComponentModel.DataAnnotations;

namespace FitnessCheck.Data.DTO.Request;

public class MedicineBallPushAttemptCreationRequestDTO
{
    /// <summary>
    /// The result of the medicine ball push attempt in centimeters.
    /// </summary>
    [Range(250, 1000, ErrorMessage = "Result in centimeters must be in range 250 - 1000.")]
    [Required(ErrorMessage = "The result in centimeters must be provided.")]
    public required uint ResultInCentimeters { get; set; }
}