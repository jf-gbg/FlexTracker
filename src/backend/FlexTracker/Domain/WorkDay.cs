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

        if (_workSlots.Any(existingWorkSlot => Overlaps(existingWorkSlot, workSlot)))
        {
            throw new ArgumentException("Work slots on the same day must not overlap.", nameof(workSlot));
        }

        _workSlots.Add(workSlot);
    }

    public TimeSpan GetWorkedDuration() => _workSlots.Aggregate(TimeSpan.Zero, (total, slot) => total + slot.GetWorkedDuration());

    private static bool Overlaps(WorkSlot left, WorkSlot right)
    {
        return left.StartTime < right.EndTime && right.StartTime < left.EndTime;
    }
}
