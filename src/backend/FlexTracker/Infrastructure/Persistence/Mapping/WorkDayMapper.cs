using FlexTracker.Domain;
using FlexTracker.Infrastructure.Persistence.Models;

namespace FlexTracker.Infrastructure.Persistence.Mapping;

internal static class WorkDayMapper
{
    public static WorkDay ToDomain(WorkDayRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);

        var workDay = new WorkDay(record.Date);

        foreach (var slotRecord in record.WorkSlots.OrderBy(workSlot => workSlot.StartTime))
        {
            LunchBreak? lunchBreak = null;

            if (slotRecord.LunchStartTime.HasValue || slotRecord.LunchEndTime.HasValue)
            {
                if (!slotRecord.LunchStartTime.HasValue || !slotRecord.LunchEndTime.HasValue)
                {
                    throw new InvalidOperationException("Lunch break columns must both be set or both be null.");
                }

                lunchBreak = new LunchBreak(slotRecord.LunchStartTime.Value, slotRecord.LunchEndTime.Value);
            }

            workDay.AddWorkSlot(new WorkSlot(slotRecord.StartTime, slotRecord.EndTime, lunchBreak));
        }

        return workDay;
    }

    public static WorkDayRecord ToRecord(WorkDay workDay)
    {
        ArgumentNullException.ThrowIfNull(workDay);

        var record = new WorkDayRecord
        {
            Date = workDay.Date
        };

        foreach (var workSlot in workDay.WorkSlots)
        {
            record.WorkSlots.Add(new WorkSlotRecord
            {
                WorkDay = record,
                StartTime = workSlot.StartTime,
                EndTime = workSlot.EndTime,
                LunchStartTime = workSlot.LunchBreak?.StartTime,
                LunchEndTime = workSlot.LunchBreak?.EndTime
            });
        }

        return record;
    }
}
