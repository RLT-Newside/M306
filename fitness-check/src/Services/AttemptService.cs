using System.Numerics;
using FitnessCheck.Data;
using FitnessCheck.Data.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using FitnessCheck.Data.DTO.Response;
using Microsoft.Extensions.Options;
using FitnessCheck.Utils;

namespace FitnessCheck.Services;

public class AttemptService(
    FitnessCheckDbContext _dbContext,
    IPointsCalculator _pointsCalculator,
    IBestAttemptService _bestAttemptService,
    IMapper _mapper,
    IOptions<MaxAllowedAttemptsOptions> maxAllowedAttemptsOptions
) : IAttemptService
{
    private readonly MaxAllowedAttemptsOptions _maxAllowedAttemptsOptions = maxAllowedAttemptsOptions.Value;

    /// <inheritdoc/>
    public async Task<TResponse?> AddAttemptAsync<TResultValue, TEntity, TResponse>(TResultValue value, (Guid userId, string username, char gender, Cohort cohort) userData, EFoot? foot = null)
        where TResultValue : INumber<TResultValue>
        where TEntity : DisciplineAttempt<TResultValue>, new()
        where TResponse : DisciplineAttemptResponseDTO
    {
        // Determine the next attempt number for this user/discipline/cohort
        var attemptNumber = GetAttemptNumber<TEntity, TResultValue>(userData.userId, userData.cohort.Id, foot);

        // Create a new attempt entity and populate its properties
        var attempt = new TEntity
        {
            AttemptNumber = attemptNumber,
            MomentUtc = DateTime.UtcNow,
            UserId = userData.userId,
            Cohort = userData.cohort,
            Gender = userData.gender
        };

        // For OneLegStand, set the foot (left/right) if provided
        if (attempt is OneLegStandAttempt oneLegStandAttempt && foot is not null)
        {
            oneLegStandAttempt.Foot = (EFoot)foot;
        }

        // Set the result value for the attempt
        attempt.SetResultValue(value);

        // Add the attempt to the database
        _dbContext.Set<TEntity>().Add(attempt);
        await _dbContext.SaveChangesAsync();

        // Find the current BestAttempt for this user/cohort/discipline
        var disciplineName = typeof(TEntity).Name.Replace("Attempt", "");
        var bestAttempt = await _dbContext.BestAttempts
            .FirstOrDefaultAsync(entry => entry.UserId == userData.userId
                                          && entry.CohortId == userData.cohort.Id
                                          && entry.Discipline == disciplineName);

        bool isBetter;
        // If the attempt implements IComparable, use it to compare with the best attempt
        if (attempt is IComparable<TEntity> comparable && bestAttempt != null)
        {
            var bestAttemptEntity = new TEntity();
            bestAttemptEntity.SetResultValue((TResultValue)Convert.ChangeType(bestAttempt.Result, typeof(TResultValue)));
            isBetter = comparable.CompareTo(bestAttemptEntity) > 0;
        }
        // Special logic for OneLegStand: sum left and right foot
        else if (attempt is OneLegStandAttempt latestOneLegStandAttempt)
        {
            var left = bestAttempt?.LeftFootResult ?? 0;
            var right = bestAttempt?.RightFootResult ?? 0;
            var newLeft = latestOneLegStandAttempt.Foot == EFoot.Left ? latestOneLegStandAttempt.ResultInSeconds : left;
            var newRight = latestOneLegStandAttempt.Foot == EFoot.Right ? latestOneLegStandAttempt.ResultInSeconds : right;
            var newSum = newLeft + newRight;
            var bestSum = left + right;
            isBetter = newSum > bestSum;
        }
        // If there is no best attempt yet, this is the best by default
        else
        {
            isBetter = bestAttempt == null;
        }

        // If this attempt is better, update the BestAttempt entity
        if (isBetter)
        {
            if (bestAttempt == null)
            {
                bestAttempt = new BestAttempt
                {
                    UserId = userData.userId,
                    CohortId = userData.cohort.Id,
                    Discipline = disciplineName,
                };
                _dbContext.BestAttempts.Add(bestAttempt);
            }

            // For OneLegStand, update left/right foot and sum
            if (attempt is OneLegStandAttempt bestOneLegStandAttempt)
            {
                if (bestOneLegStandAttempt.Foot == EFoot.Left)
                {
                    bestAttempt.LeftFootResult = bestOneLegStandAttempt.ResultInSeconds;
                }
                else if (bestOneLegStandAttempt.Foot == EFoot.Right)
                {
                    bestAttempt.RightFootResult = bestOneLegStandAttempt.ResultInSeconds;
                }
                var left = bestAttempt.LeftFootResult ?? 0;
                var right = bestAttempt.RightFootResult ?? 0;
                bestAttempt.Result = left + right;
            }
            // For other disciplines, just set the result
            else
            {
                bestAttempt.Result = Convert.ToSingle(value);
            }

            // Calculate and update points for the best attempt
            var pointsFilter = attempt.GetPointsFilter((TResultValue)Convert.ChangeType(bestAttempt.Result, typeof(TResultValue)), userData.gender);
            var points = _pointsCalculator.CalculatePoints(pointsFilter);
            bestAttempt.Points = points;

            // Set moment (UTC) of best attemt
            bestAttempt.MomentUtc = attempt.MomentUtc;

            await _dbContext.SaveChangesAsync();
        }

        // Return the mapped response DTO
        return _mapper.Map<TResponse>(attempt);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAttemptAsync<TEntity, TResultValue>(Guid attemptId, Guid userId)
        where TEntity : DisciplineAttempt<TResultValue>, new()
        where TResultValue : INumber<TResultValue>
    {
        // Find the attempt to delete
        var attemptToDelete = await _dbContext.Set<TEntity>().FindAsync(attemptId);

        // If not found, return false
        if (attemptToDelete is null)
        {
            return false;
        }

        // Only allow deletion if the user owns the attempt
        if (!attemptToDelete.UserId.Equals(userId))
        {
            return false;
        }

        var cohortId = attemptToDelete.CohortId;

        // Decrement attempt numbers for all later attempts in the same cohort
        var attemptsToUpdate = await _dbContext.Set<TEntity>().Where(attempt => attempt.UserId == userId
                                                                                && attempt.CohortId == attemptToDelete.CohortId
                                                                                && attempt.AttemptNumber > attemptToDelete.AttemptNumber).ToListAsync();
        foreach (var attempt in attemptsToUpdate)
        {
            attempt.AttemptNumber--;
        }

        // Recalculate or remove BestAttempt for this discipline/cohort/user BEFORE deletion
        var disciplineName = typeof(TEntity).Name.Replace("Attempt", "");
        
        var bestAttempt = await _dbContext.BestAttempts
            .FirstOrDefaultAsync(entry => entry.UserId == userId
                                         && entry.CohortId == attemptToDelete.CohortId
                                         && entry.Discipline == disciplineName);

        // Remove the attempt from the database
        _dbContext.Remove(attemptToDelete);

        // Check if there will be remaining attempts after the deletion
        var remainingAttemptsCount = await _dbContext.Set<TEntity>()
            .Where(a => a.UserId == userId && a.CohortId == attemptToDelete.CohortId && a.Id != attemptToDelete.Id)
            .CountAsync();

        if (remainingAttemptsCount > 0)
        {
            // If there are still attempts, recalculate the best
            if (bestAttempt is null)
            {
                bestAttempt = new BestAttempt
                {
                    UserId = userId,
                    CohortId = cohortId,
                    Discipline = disciplineName
                };
                _dbContext.BestAttempts.Add(bestAttempt);
            }

            // Save changes to commit the deletion first, then recalculate
            await _dbContext.SaveChangesAsync();
            
            // Use the BestAttemptService to recalculate the best attempt
            var newBestAttempt = await _bestAttemptService.CalculateBestAttemptAsync<TEntity, TResultValue>(userId, cohortId);
            if (newBestAttempt is not null)
            {
                bestAttempt.Result = newBestAttempt.Result;
                bestAttempt.Points = newBestAttempt.Points;
                bestAttempt.MomentUtc = newBestAttempt.MomentUtc;
                bestAttempt.LastCalculated = DateTime.UtcNow;

                // For OneLegStand, update left/right foot
                if (typeof(TEntity) == typeof(OneLegStandAttempt))
                {
                    bestAttempt.LeftFootResult = newBestAttempt.LeftFootResult;
                    bestAttempt.RightFootResult = newBestAttempt.RightFootResult;
                }
            }
        }
        // If no attempts remain, remove the BestAttempt entity
        else if (bestAttempt != null)
        {
            _dbContext.BestAttempts.Remove(bestAttempt);
        }

        // Save the final BestAttempt changes
        await _dbContext.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Determines the next attempt number for a user, discipline, and cohort in the current school year. Handles OneLegStand per-foot logic.
    /// </summary>
    /// <typeparam name="TEntity">The entity type for the discipline attempt.</typeparam>
    /// <typeparam name="TResultValue">The numeric type of the result value for the discipline.</typeparam>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="cohortId">The ID of the cohort.</param>
    /// <param name="foot">The foot (left/right) for OneLegStand, if applicable.</param>
    /// <returns>The next attempt number for the user and discipline in the current school year.</returns>
    private byte GetAttemptNumber<TEntity, TResultValue>(Guid userId, Guid cohortId, EFoot? foot = null)
        where TEntity : DisciplineAttempt<TResultValue>
        where TResultValue : INumber<TResultValue>
    {
        // Get the max allowed attempts for this discipline
        var maxAllowedAttempts = _maxAllowedAttemptsOptions.GetForDisciplineAttempt<TEntity, TResultValue>();

        // Get the start of the current school year
        var currentSchoolYearStart = SchoolYearUtils.GetSchoolYearStart();

        // Build a predicate for filtering attempts in the current school year
        Func<TEntity, bool> where = foot is not null && typeof(TEntity) == typeof(OneLegStandAttempt)
            ? (TEntity attempt) => attempt.UserId == userId &&
                                  attempt.CohortId == cohortId &&
                                  attempt.MomentUtc >= currentSchoolYearStart &&
                                  (attempt as OneLegStandAttempt)!.Foot == foot
            : (TEntity attempt) => attempt.UserId == userId &&
                                  attempt.CohortId == cohortId &&
                                  attempt.MomentUtc >= currentSchoolYearStart;

        // Find the last attempt number for this user/discipline/cohort in the current school year
        var lastAttemptNumber = _dbContext.Set<TEntity>()
                               .Where(where)
                               .OrderByDescending(attempt => attempt.AttemptNumber)
                               .Select(attempt => attempt.AttemptNumber)
                               .FirstOrDefault();

        // The next attempt number is last + 1
        var attemptNumber = (byte)(lastAttemptNumber + 1);

        // Throw if the user has reached the max allowed attempts
        if (attemptNumber > maxAllowedAttempts)
        {
            throw new ArgumentOutOfRangeException(null, $"Die maximale Anzahl Versuche für diese Disziplin ({maxAllowedAttempts}) ist erreicht.");
        }

        return attemptNumber;
    }
}