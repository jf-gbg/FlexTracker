namespace FlexTracker.Domain;

public sealed class WorkSlot
{
    public WorkSlot(TimeOnly startTime, TimeOnly endTime, LunchBreak? lunchBreak = null)
    {
        if (endTime <= startTime)
        {
            throw new ArgumentException("Work slot end time must be after start time.", nameof(endTime));
        }

        if (lunchBreak is not null)
        {
            if (lunchBreak.StartTime < startTime || lunchBreak.EndTime > endTime)
            {
                throw new ArgumentException("Lunch break must be inside the work slot.", nameof(lunchBreak));
            }
        }

        StartTime = startTime;
        EndTime = endTime;
        LunchBreak = lunchBreak;
    }

    public TimeOnly StartTime { get; }

    public TimeOnly EndTime { get; }

    public LunchBreak? LunchBreak { get; }
}
