using FlexTracker.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace FlexTracker.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<TimeEntryDbo> TimeEntries => Set<TimeEntryDbo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<TimeEntryDbo>(builder =>
        {
            builder.ToTable("TimeEntries");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Date).HasColumnType("TEXT");
            builder.Property(x => x.StartTime).HasColumnType("TEXT");
            builder.Property(x => x.EndTime).HasColumnType("TEXT");
            builder.Property(x => x.LunchStartTime).HasColumnType("TEXT");
            builder.Property(x => x.LunchEndTime).HasColumnType("TEXT");
        });
    }
}
