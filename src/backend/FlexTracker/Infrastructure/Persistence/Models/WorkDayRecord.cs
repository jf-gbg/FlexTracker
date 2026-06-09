namespace FlexTracker.Infrastructure.Persistence.Models;

public sealed class WorkDayRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateOnly Date { get; set; }

    public List<WorkSlotRecord> WorkSlots { get; } = [];
}
