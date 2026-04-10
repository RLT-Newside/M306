using System.Numerics;
using FitnessCheck.Data;
using FitnessCheck.Data.Entities;
using FitnessCheck.Data.DTO.Response;

namespace FitnessCheck.Services;

public interface IAttemptService
{

    /// <summary>
    /// Adds a new attempt for the specified user, discipline, and value. Handles OneLegStand special logic and updates the BestAttempt cache if needed.
    /// </summary>
    /// <typeparam name="TResultValue">The numeric type of the result value for the discipline.</typeparam>
    /// <typeparam name="TEntity">The entity type for the discipline attempt.</typeparam>
    /// <typeparam name="TResponse">The DTO type for the response.</typeparam>
    /// <param name="value">The result value for the attempt.</param>
    /// <param name="userData">Tuple containing userId, username, gender, and cohort.</param>
    /// <param name="foot">The foot (left/right) for OneLegStand, if applicable.</param>
    /// <returns>The created attempt mapped to the response DTO.</returns>
    Task<TResponse?> AddAttemptAsync<TResultValue, TEntity, TResponse>(TResultValue value, (Guid userId, string username, char gender, Cohort cohort) userData, EFoot? foot = null)
        where TResultValue : INumber<TResultValue>
        where TEntity : DisciplineAttempt<TResultValue>, new()
        where TResponse : DisciplineAttemptResponseDTO;

    /// <summary>
    /// Deletes an attempt by ID for the specified user and discipline. Updates attempt numbers and recalculates or removes BestAttempt as needed.
    /// </summary>
    /// <typeparam name="TEntity">The entity type for the discipline attempt.</typeparam>
    /// <typeparam name="TResultValue">The numeric type of the result value for the discipline.</typeparam>
    /// <param name="attemptId">The ID of the attempt to delete.</param>
    /// <param name="userId">The ID of the user who owns the attempt.</param>
    /// <returns>True if the attempt was deleted, false otherwise.</returns>
    Task<bool> DeleteAttemptAsync<TEntity, TResultValue>(Guid attemptId, Guid userId)
        where TEntity : DisciplineAttempt<TResultValue>, new()
        where TResultValue : INumber<TResultValue>;
}
