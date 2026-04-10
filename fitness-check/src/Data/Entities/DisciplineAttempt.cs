using System.Numerics;
using FitnessCheckModels;

namespace FitnessCheck.Data.Entities;

public abstract class DisciplineAttempt
{
    public Guid Id { get; set; }
    public byte AttemptNumber { get; set; }
    public DateTime MomentUtc { get; set; }
    public Guid UserId { get; set; }
    public char Gender { get; set; }
    public Guid CohortId { get; set; }
    public Cohort Cohort { get; set; } = null!;

}

public abstract class DisciplineAttempt<TResultValue> : DisciplineAttempt where TResultValue : INumber<TResultValue>
{
    public abstract void SetResultValue(TResultValue value);
    public abstract TResultValue GetResultValue();
    public abstract Func<ResultsCalculation, bool> GetPointsFilter(TResultValue result, char gender);

    protected T GetPropertyValue<T>(ResultsCalculation obj, string propertyName) where T : INumber<T>
    {
        var property = typeof(ResultsCalculation).GetProperty(propertyName)
                       ?? throw new ArgumentException($"There is no property named '{propertyName}' in class '{nameof(ResultsCalculation)}'.", nameof(propertyName));
        var propertyValue = property.GetValue(obj)
                            ?? throw new ArgumentException($"The value for property '{propertyName}' is null.", propertyName);

        return (T)Convert.ChangeType(propertyValue, typeof(T));
    }
}