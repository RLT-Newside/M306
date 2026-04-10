namespace FitnessCheck.Data;

/// <summary>
/// Configuration options for discipline-specific data recorder settings.
/// Contains gender-specific ranges and configuration for each fitness discipline.
/// </summary>
public class DisciplineConfigurationOptions
{
    /// <summary>
    /// The configuration path for discipline configuration options.
    /// </summary>
    public const string ConfigurationPath = "FitnessCheck:DisciplineConfiguration";

    /// <summary>
    /// Configuration for Core Strength discipline.
    /// </summary>
    public DisciplineConfig CoreStrength { get; set; } = new();

    /// <summary>
    /// Configuration for Medicine Ball Push discipline.
    /// </summary>
    public DisciplineConfig MedicineBallPush { get; set; } = new();

    /// <summary>
    /// Configuration for Standing Long Jump discipline.
    /// </summary>
    public DisciplineConfig StandingLongJump { get; set; } = new();

    /// <summary>
    /// Configuration for One Leg Stand discipline.
    /// </summary>
    public DisciplineConfig OneLegStand { get; set; } = new();

    /// <summary>
    /// Configuration for Shuttle Run discipline.
    /// </summary>
    public DisciplineConfig ShuttleRun { get; set; } = new();

    /// <summary>
    /// Configuration for Twelve Minutes Run discipline.
    /// </summary>
    public DisciplineConfig TwelveMinutesRun { get; set; } = new();

    /// <summary>
    /// Gets the configuration for a specific discipline by name.
    /// </summary>
    /// <param name="disciplineName">The name of the discipline.</param>
    /// <returns>The discipline configuration or null if not found.</returns>
    public DisciplineConfig? GetConfigurationForDiscipline(string disciplineName)
    {
        return disciplineName switch
        {
            "CoreStrength" => CoreStrength,
            "MedicineBallPush" => MedicineBallPush,
            "StandingLongJump" => StandingLongJump,
            "OneLegStand" => OneLegStand,
            "ShuttleRun" => ShuttleRun,
            "TwelveMinutesRun" => TwelveMinutesRun,
            _ => null
        };
    }
}

/// <summary>
/// Configuration for a specific discipline with gender-specific data recorder settings.
/// </summary>
public class DisciplineConfig
{
    /// <summary>
    /// Data recorder configuration for male users.
    /// </summary>
    public DataRecorderConfig Male { get; set; } = new();

    /// <summary>
    /// Data recorder configuration for female users.
    /// </summary>
    public DataRecorderConfig Female { get; set; } = new();

    /// <summary>
    /// Gets the configuration for the specified gender.
    /// </summary>
    /// <param name="gender">The gender ('m' for male, 'f' for female).</param>
    /// <returns>The data recorder configuration for the gender.</returns>
    public DataRecorderConfig GetConfigurationForGender(char gender)
    {
        return gender switch
        {
            'm' => Male,
            'f' => Female,
            _ => Male // Default to male configuration
        };
    }
}

/// <summary>
/// Data recorder configuration with ranges and settings.
/// </summary>
public class DataRecorderConfig
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