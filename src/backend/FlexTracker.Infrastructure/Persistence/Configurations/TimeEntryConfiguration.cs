using FlexTracker.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexTracker.Infrastructure.Persistence.Configurations;

public sealed class TimeEntryConfiguration : IEntityTypeConfiguration<TimeEntryDbo>
{
    public void Configure(EntityTypeBuilder<TimeEntryDbo> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Date);
        builder.Property(x => x.StartTime);
        builder.Property(x => x.EndTime);
        builder.Property(x => x.LunchStartTime);
        builder.Property(x => x.LunchEndTime);
    }
}
