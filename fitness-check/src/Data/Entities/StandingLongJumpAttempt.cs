using FitnessCheckModels;

namespace FitnessCheck.Data.Entities;

public class StandingLongJumpAttempt : DisciplineAttempt<uint>, IComparable<StandingLongJumpAttempt>
{
    public uint ResultInCentimeters { get; set; }

    public override void SetResultValue(uint value)
    {
        ResultInCentimeters = value;
    }

    public override uint GetResultValue()
    {
        return ResultInCentimeters;
    }

    public int CompareTo(StandingLongJumpAttempt? other)
    {
        if (other == null) return 1;

        // Higher is better
        return ResultInCentimeters.CompareTo(other.ResultInCentimeters);
    }

    public override Func<ResultsCalculation, bool> GetPointsFilter(uint result, char gender)
    {
        return resultsCalculation => GetPropertyValue<int>(resultsCalculation, nameof(ResultsCalculation.StandingLongJump)) <= result
                                     && resultsCalculation.Gender == gender;
    }
}
