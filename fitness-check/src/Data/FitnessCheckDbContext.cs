using System.Text.Json;
using FitnessCheck.Data.Entities;
using FitnessCheckModels;
using Microsoft.EntityFrameworkCore;

namespace FitnessCheck.Data;

public class FitnessCheckDbContext : DbContext
{
    public virtual DbSet<CoreStrengthAttempt> CoreStrengthAttempts { get; set; }
    public virtual DbSet<MedicineBallPushAttempt> MedicineBallPushAttempts { get; set; }
    public virtual DbSet<StandingLongJumpAttempt> StandingLongJumpAttempts { get; set; }
    public virtual DbSet<OneLegStandAttempt> OneLegStandAttempts { get; set; }
    public virtual DbSet<ShuttleRunAttempt> ShuttleRunAttempts { get; set; }
    public virtual DbSet<TwelveMinutesRunAttempt> TwelveMinutesRunAttempts { get; set; }
    public virtual DbSet<ResultsCalculation> Results { get; set; }
    public virtual DbSet<Cohort> Cohorts { get; set; }
    public virtual DbSet<BestAttempt> BestAttempts { get; set; }

    public FitnessCheckDbContext() : base()
    {

    }

    public FitnessCheckDbContext(DbContextOptions<FitnessCheckDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Seed data
        SeedResultsCalculation(modelBuilder);
    }

    public override int SaveChanges()
    {
        UpdateBestAttemptTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateBestAttemptTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateBestAttemptTimestamps()
    {
        var entries = ChangeTracker.Entries<BestAttempt>()
                                   .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            entry.Entity.LastCalculated = DateTime.UtcNow;
        }
    }

    private void SeedResultsCalculation(ModelBuilder modelBuilder)
    {
        var resultsCalculation = DeserializeJSON<ResultsCalculation>("Data/ResultsCalculationData.json");
        modelBuilder.Entity<ResultsCalculation>().HasData(resultsCalculation);
    }

    private IEnumerable<T> DeserializeJSON<T>(string jsonFileName)
    {
        var json = File.ReadAllText(jsonFileName);

#pragma warning disable CS8603 // Possible null reference return.
        return JsonSerializer.Deserialize<IEnumerable<T>>(json);
#pragma warning restore CS8603 // Possible null reference return.
    }
}