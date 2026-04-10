namespace FitnessCheck.Data.DTO.Response;

/// <summary>
/// Response DTO for data recorder configuration with gender-specific ranges and settings.
/// </summary>
public class DataRecorderConfigResponseDTO
{
    /// <summary>
    /// The minimum value for the data recorder range.
    /// </summary>
    public double MinimumValue { get; set; }
    
    /// <summary>
    /// The maximum value for the data recorder range.
    /// </summary>
    public double MaximumValue { get; set; }
    
    /// <summary>
    /// The interval between major ticks on the recorder.
    /// </summary>
    public double Interval { get; set; }
    
    /// <summary>
    /// The initial/default value when the recorder is opened.
    /// </summary>
    public double InitialValue { get; set; }
    
    /// <summary>
    /// The step value for increment/decrement operations.
    /// </summary>
    public double StepperValue { get; set; }
    
    /// <summary>
    /// The number of minor ticks per interval.
    /// </summary>
    public int TicksPerInterval { get; set; }
}