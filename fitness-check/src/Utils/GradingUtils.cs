namespace FitnessCheck.Utils;

public static class GradingUtils
{
    /// <summary>
    /// Default value for the UED rating threshold (can be overridden in appsettings.json).
    /// </summary>
    public const short UED_THRESHOLD = 22;

    /// <summary>
    /// Default value for the UEE rating threshold (can be overridden in appsettings.json).
    /// </summary>
    public const short UEE_THRESHOLD = 16;

    /// <summary>
    /// Default value for the EE rating threshold (can be overridden in appsettings.json).
    /// </summary>
    public const short EE_THRESHOLD = 7;
    /// <summary>
    /// Determines the rating string (UED, UEE, EE, TE) based on total points and configuration thresholds.
    /// </summary>
    /// <param name="totalPoints">The total points to grade.</param>
    /// <returns>The rating string.</returns>
    public static string GetRating(uint points)
    {

        if (points >= UED_THRESHOLD)
            return "UED";
        if (points >= UEE_THRESHOLD)
            return "UEE";
        if (points >= EE_THRESHOLD)
            return "EE";
        return "TE";
    }
}