namespace FlexTracker.Domain;

public sealed class LunchBreak
{
    public LunchBreak(TimeOnly startTime, TimeOnly endTime)
    {
        if (endTime <= startTime)
        {
            throw new ArgumentException("Lunch end time must be after lunch start time.", nameof(endTime));
        }

        StartTime = startTime;
        EndTime = endTime;
    }

    public TimeOnly StartTime { get; }

    public TimeOnly EndTime { get; }

    public TimeSpan Duration => EndTime - StartTime;
}
