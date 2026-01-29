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
    }
}
