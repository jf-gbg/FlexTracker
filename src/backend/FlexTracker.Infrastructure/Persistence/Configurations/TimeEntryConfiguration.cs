using FlexTracker.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexTracker.Infrastructure.Persistence.Configurations;

public sealed class TimeEntryConfiguration : IEntityTypeConfiguration<TimeEntryDbo>
{
    public void Configure(EntityTypeBuilder<TimeEntryDbo> builder)
    {
        builder.ToTable("TimeEntries");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Date).HasColumnType("TEXT");
        builder.Property(x => x.StartTime).HasColumnType("TEXT");
        builder.Property(x => x.EndTime).HasColumnType("TEXT");
        builder.Property(x => x.LunchStartTime).HasColumnType("TEXT");
        builder.Property(x => x.LunchEndTime).HasColumnType("TEXT");
    }
}
