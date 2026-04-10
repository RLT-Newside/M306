using FitnessCheckModels;

namespace FitnessCheck.Data.Entities;

public class OneLegStandAttempt : DisciplineAttempt<uint>
{
    public EFoot Foot { get; set; }
    public uint ResultInSeconds { get; set; }

    public override void SetResultValue(uint value)
    {
        ResultInSeconds = value;
    }

    public override uint GetResultValue()
    {
        return ResultInSeconds;
    }

    public override Func<ResultsCalculation, bool> GetPointsFilter(uint result, char gender)
    {
        return resultsCalculation => GetPropertyValue<int>(resultsCalculation, nameof(ResultsCalculation.OneLegStand)) <= result
                                     && resultsCalculation.Gender == gender;
    }
}
