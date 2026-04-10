namespace FitnessCheck.Utils;

/// <summary>
/// Utility methods for school year calculations.
/// </summary>
public static class SchoolYearUtils
{
    // School year starts on August 1st
    public const int SchoolYearStartMonth = 8;
    public const int SchoolYearStartDay = 1;

    /// <summary>
    /// Calculates the start date of the school year.
    /// If no school year is provided, the calculation is based on the current date.
    /// </summary>
    /// <param name="schoolYear">Optional school year (e.g., 2025 for 2025/26)</param>
    /// <returns>DateTime representing the start of the current school year.</returns>
    public static DateTime GetSchoolYearStart(ushort? schoolYear = null)
    {
        if (schoolYear is not null)
        {
            return new DateTime((int)schoolYear, SchoolYearStartMonth, SchoolYearStartDay, 0, 0, 0);
        }

        var currentDate = DateTime.UtcNow;
        return new DateTime(
            currentDate.Month >= SchoolYearStartMonth ? currentDate.Year : currentDate.Year - 1,
            SchoolYearStartMonth,
            SchoolYearStartDay,
            0,
            0,
            0
        );
    }

    /// <summary>
    /// Determines the school year (e.g., 2025 for 2025/26) for a given date.
    /// If no date is provided, uses the current UTC date.
    /// </summary>
    /// <param name="date">The date to evaluate (optional).</param>
    /// <returns>The school year as a ushort (e.g., 2025 for 2025/26).</returns>
    public static ushort GetSchoolYearForDate(DateTime? date = null)
    {
        var d = date ?? DateTime.UtcNow;
        return (ushort)(d.Month >= SchoolYearStartMonth ? d.Year : d.Year - 1);
    }
}
