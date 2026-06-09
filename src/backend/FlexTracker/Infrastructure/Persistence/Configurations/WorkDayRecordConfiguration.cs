using FlexTracker.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexTracker.Infrastructure.Persistence.Configurations;

internal sealed class WorkDayRecordConfiguration : IEntityTypeConfiguration<WorkDayRecord>
{
    public void Configure(EntityTypeBuilder<WorkDayRecord> builder)
    {
        builder.ToTable("work_days");

        builder.HasKey(workDay => workDay.Id);

        builder.Property(workDay => workDay.Id)
            .HasColumnName("id");

        builder.Property(workDay => workDay.Date)
            .HasColumnName("date")
            .IsRequired();

        builder.HasMany(workDay => workDay.WorkSlots)
            .WithOne(workSlot => workSlot.WorkDay)
            .HasForeignKey(workSlot => workSlot.WorkDayId)
            .IsRequired();
    }
}
