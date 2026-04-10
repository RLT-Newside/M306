using FitnessCheckModels;

namespace FitnessCheck.Data.Entities;

public class CoreStrengthAttempt : DisciplineAttempt<uint>, IComparable<CoreStrengthAttempt>
{
    public uint ResultInSeconds { get; set; }

    public override void SetResultValue(uint value)
    {
        ResultInSeconds = value;
    }

    public override uint GetResultValue()
    {
        return ResultInSeconds;
    }

    public int CompareTo(CoreStrengthAttempt? other)
    {
        if (other == null) return 1;

        // Higher is better
        return ResultInSeconds.CompareTo(other.ResultInSeconds);
    }

    public override Func<ResultsCalculation, bool> GetPointsFilter(uint result, char gender)
    {
        return resultsCalculation => GetPropertyValue<int>(resultsCalculation, nameof(ResultsCalculation.CoreStrength)) <= result
                                     && resultsCalculation.Gender == gender;
    }
}
