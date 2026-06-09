namespace FlexTracker.Infrastructure.Persistence.Models;

public sealed class WorkSlotRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid WorkDayId { get; set; }

    public WorkDayRecord WorkDay { get; set; } = null!;

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public TimeOnly? LunchStartTime { get; set; }

    public TimeOnly? LunchEndTime { get; set; }
}
