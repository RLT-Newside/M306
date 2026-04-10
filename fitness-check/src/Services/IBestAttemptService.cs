
using FitnessCheck.Data.Entities;
using FitnessCheck.Data.DTO.Response;

namespace FitnessCheck.Services;

/// <summary>
/// Service for calculating and retrieving best attempts and annual performance for fitness check disciplines.
/// </summary>
public interface IBestAttemptService
{
    /// <summary>
    /// Calculates the best attempt for a given user, cohort, and discipline by evaluating all attempts and applying discipline-specific logic.
    /// </summary>
    /// <typeparam name="TDiscipline">The type of the discipline attempt entity.</typeparam>
    /// <typeparam name="TResultValue">The numeric type of the result value for the discipline.</typeparam>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cohortId">The unique identifier of the cohort.</param>
    /// <returns>The calculated <see cref="BestAttempt"/> for the user, cohort, and discipline, or <c>null</c> if no attempts exist.</returns>
    Task<BestAttempt?> CalculateBestAttemptAsync<TDiscipline, TResultValue>(Guid userId, Guid cohortId)
        where TDiscipline : DisciplineAttempt<TResultValue>, new()
        where TResultValue : System.Numerics.INumber<TResultValue>;

    /// <summary>
    /// Calculates the best attempt for all supported disciplines for a given user and cohort.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cohortId">The unique identifier of the cohort.</param>
    /// <returns>A list of <see cref="BestAttempt"/> objects, one for each supported discipline.</returns>
    Task<List<BestAttempt>> GetAllBestAttemptsAsync(Guid userId, Guid cohortId);

    /// <summary>
    /// Retrieves the annual performance (best result and points per school year) for a user and discipline.
    /// </summary>
    /// <typeparam name="TDiscipline">The type of the discipline attempt entity.</typeparam>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A list of <see cref="AnnualPerformanceResponseDTO"/> objects, one for each school year with results.</returns>
    Task<List<AnnualPerformanceResponseDTO>> GetAnnualPerformanceAsync<TDiscipline>(Guid userId)
        where TDiscipline : DisciplineAttempt;
}
