using AutoMapper;
using FitnessCheck.Data.Entities;
using Microsoft.Extensions.Options;

namespace FitnessCheck.Data;

/// <summary>
/// AutoMapper value resolver that calculates the deletion deadline for discipline attempts.
/// The deadline is calculated by adding the configured offset in seconds to the attempt's creation time.
/// </summary>
public class DeletionDeadlineResolver : IValueResolver<DisciplineAttempt, object, DateTime>
{
    private readonly DeletionDeadlineOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeletionDeadlineResolver"/> class.
    /// </summary>
    /// <param name="options">The deletion deadline configuration options.</param>
    public DeletionDeadlineResolver(IOptions<DeletionDeadlineOptions> options)
    {
        _options = options.Value;
    }

    /// <summary>
    /// Resolves the deletion deadline by adding the configured offset to the attempt's creation time.
    /// </summary>
    /// <param name="source">The source discipline attempt entity.</param>
    /// <param name="destination">The destination object (not used).</param>
    /// <param name="destMember">The destination member (not used).</param>
    /// <param name="context">The AutoMapper resolution context.</param>
    /// <returns>The calculated deletion deadline as a UTC DateTime.</returns>
    public DateTime Resolve(DisciplineAttempt source, object destination, DateTime destMember, ResolutionContext context)
    {
        return source.MomentUtc.AddSeconds(_options.OffsetInSeconds);
    }
}