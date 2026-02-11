namespace FlexTracker.Domain;

public sealed class TimeEntry
{
    public DateOnly Date { get; }
    public TimeOnly StartTime { get; }
    public TimeOnly EndTime { get; }
    public TimeOnly? LunchStartTime { get; }
    public TimeOnly? LunchEndTime { get; }

    private TimeEntry(
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        TimeOnly? lunchStartTime,
        TimeOnly? lunchEndTime)
    {
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
        LunchStartTime = lunchStartTime;
        LunchEndTime = lunchEndTime;
    }

    public static TimeEntry? Create(
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        TimeOnly? lunchStartTime,
        TimeOnly? lunchEndTime,
        out IReadOnlyList<ValidationError> errors)
    {
        var validationErrors = new List<ValidationError>();

        if (endTime <= startTime)
        {
            validationErrors.Add(new ValidationError("endTime", "End time must be after start time."));
        }

        var hasLunchStart = lunchStartTime.HasValue;
        var hasLunchEnd = lunchEndTime.HasValue;
        if (hasLunchStart != hasLunchEnd)
        {
            validationErrors.Add(new ValidationError("lunchStartTime", "Lunch start and end must both be provided."));
            validationErrors.Add(new ValidationError("lunchEndTime", "Lunch start and end must both be provided."));
        }

        if (hasLunchStart && hasLunchEnd)
        {
            if (lunchEndTime <= lunchStartTime)
            {
                validationErrors.Add(new ValidationError("lunchEndTime", "Lunch end must be after lunch start."));
            }

            if (lunchStartTime < startTime || lunchEndTime > endTime)
            {
                validationErrors.Add(new ValidationError("lunchStartTime", "Lunch must be within work interval."));
                validationErrors.Add(new ValidationError("lunchEndTime", "Lunch must be within work interval."));
            }
        }

        if (validationErrors.Count > 0)
        {
            errors = validationErrors;
            return null;
        }

        errors = Array.Empty<ValidationError>();
        return new TimeEntry(date, startTime, endTime, lunchStartTime, lunchEndTime);
    }

    public static bool HasOverlapValidationError(bool hasOverlap, out ValidationError error)
    {
        if (hasOverlap)
        {
            error = new ValidationError("timeRange", "Time entry overlaps an existing entry.");
            return true;
        }

        error = default!;
        return false;
    }

    public int GetLunchMinutes()
    {
        if (!LunchStartTime.HasValue || !LunchEndTime.HasValue)
        {
            return 0;
        }

        return (int)(LunchEndTime.Value - LunchStartTime.Value).TotalMinutes;
    }

    public int GetWorkedMinutes()
    {
        var workedMinutes = (int)(EndTime - StartTime).TotalMinutes;
        return workedMinutes - GetLunchMinutes();
    }
}
