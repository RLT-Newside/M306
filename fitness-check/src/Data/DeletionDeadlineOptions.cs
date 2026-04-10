namespace FitnessCheck.Data;

/// <summary>
/// Configuration options for deletion deadline functionality.
/// Defines how long after creation an attempt can be deleted by the user.
/// </summary>
public class DeletionDeadlineOptions
{
    /// <summary>
    /// The configuration path for deletion deadline options.
    /// </summary>
    public const string ConfigurationPath = "FitnessCheck:DeletionDeadline";

    /// <summary>
    /// The number of seconds after attempt creation during which the user can delete the attempt.
    /// Default value is 300 seconds (5 minutes).
    /// </summary>
    public int OffsetInSeconds { get; set; } = 300;
}