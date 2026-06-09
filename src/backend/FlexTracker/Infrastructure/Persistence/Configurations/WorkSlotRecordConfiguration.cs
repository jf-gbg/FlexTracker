using FlexTracker.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexTracker.Infrastructure.Persistence.Configurations;

internal sealed class WorkSlotRecordConfiguration : IEntityTypeConfiguration<WorkSlotRecord>
{
    public void Configure(EntityTypeBuilder<WorkSlotRecord> builder)
    {
        builder.ToTable("work_slots");

        builder.HasKey(workSlot => workSlot.Id);

        builder.Property(workSlot => workSlot.Id)
            .HasColumnName("id");

        builder.Property(workSlot => workSlot.WorkDayId)
            .HasColumnName("work_day_id")
            .IsRequired();

        builder.Property(workSlot => workSlot.StartTime)
            .HasColumnName("start_time")
            .IsRequired();

        builder.Property(workSlot => workSlot.EndTime)
            .HasColumnName("end_time")
            .IsRequired();

        builder.Property(workSlot => workSlot.LunchStartTime)
            .HasColumnName("lunch_start_time");

        builder.Property(workSlot => workSlot.LunchEndTime)
            .HasColumnName("lunch_end_time");
    }
}
