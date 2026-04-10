using FitnessCheckModels;

namespace FitnessCheck.Data.Entities;

public class ShuttleRunAttempt : DisciplineAttempt<uint>, IComparable<ShuttleRunAttempt>
{
    public uint ResultInMilliseconds { get; set; }

    public override void SetResultValue(uint value)
    {
        ResultInMilliseconds = value;
    }

    public override uint GetResultValue()
    {
        return ResultInMilliseconds;
    }

    public int CompareTo(ShuttleRunAttempt? other)
    {
        if (other == null) return 1;

        // Lower is better
        return other.ResultInMilliseconds.CompareTo(ResultInMilliseconds);
    }

    public override Func<ResultsCalculation, bool> GetPointsFilter(uint result, char gender)
    {
        return resultsCalculation => GetPropertyValue<int>(resultsCalculation, nameof(ResultsCalculation.ShuttleRun)) >= result
                                     && resultsCalculation.Gender == gender;
    }
}
