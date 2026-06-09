using FlexTracker.Infrastructure.Persistence.Configurations;
using FlexTracker.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace FlexTracker.Infrastructure.Persistence;

public sealed class FlexTrackerDbContext(DbContextOptions<FlexTrackerDbContext> options) : DbContext(options)
{
    public DbSet<WorkDayRecord> WorkDays => Set<WorkDayRecord>();

    public DbSet<WorkSlotRecord> WorkSlots => Set<WorkSlotRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new WorkDayRecordConfiguration());
        modelBuilder.ApplyConfiguration(new WorkSlotRecordConfiguration());
    }
}
