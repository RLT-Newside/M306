using FitnessCheckModels;

namespace FitnessCheck.Data.Entities;

public class TwelveMinutesRunAttempt : DisciplineAttempt<float>, IComparable<TwelveMinutesRunAttempt>
{
    public float ResultInRounds { get; set; }

    public override void SetResultValue(float value)
    {
        ResultInRounds = value;
    }

    public override float GetResultValue()
    {
        return ResultInRounds;
    }

    public int CompareTo(TwelveMinutesRunAttempt? other)
    {
        if (other == null) return 1;

        // Higher is better
        return ResultInRounds.CompareTo(other.ResultInRounds);
    }

    public override Func<ResultsCalculation, bool> GetPointsFilter(float result, char gender)
    {
        return resultsCalculation => GetPropertyValue<int>(resultsCalculation, nameof(ResultsCalculation.TwelveMinutesRun)) <= result
                                     && resultsCalculation.Gender == gender;
    }
}
