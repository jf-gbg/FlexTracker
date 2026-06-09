namespace FlexTracker.Domain;

public sealed class WorkDay
{
    private readonly List<WorkSlot> _workSlots = [];

    public WorkDay(DateOnly date)
    {
        Date = date;
    }

    public DateOnly Date { get; }

    public IReadOnlyList<WorkSlot> WorkSlots => _workSlots;

    public void AddWorkSlot(WorkSlot workSlot)
    {
        ArgumentNullException.ThrowIfNull(workSlot);

        _workSlots.Add(workSlot);
    }
}
