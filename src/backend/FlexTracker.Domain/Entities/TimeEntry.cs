using FlexTracker.Domain.Validation;

namespace FlexTracker.Domain.Entities;

public class TimeEntry
{
    public int Id { get; }
    public DateOnly Date { get; }
    public TimeOnly StartTime { get; }
    public TimeOnly EndTime { get; }
    public TimeOnly? LunchStartTime { get; }
    public TimeOnly? LunchEndTime { get; }

    private TimeEntry(
        int id,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        TimeOnly? lunchStartTime,
        TimeOnly? lunchEndTime)
    {
        Id = id;
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
        errors = Validate(startTime, endTime, lunchStartTime, lunchEndTime);

        if (errors.Count > 0)
            return null;

        return new TimeEntry(0, date, startTime, endTime, lunchStartTime, lunchEndTime);
    }

    public static TimeEntry Rehydrate(
        int id,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        TimeOnly? lunchStartTime,
        TimeOnly? lunchEndTime)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than zero.");

        var errors = Validate(startTime, endTime, lunchStartTime, lunchEndTime);
        if (errors.Count > 0)
            throw new InvalidOperationException("Cannot rehydrate invalid time entry data.");

        return new TimeEntry(id, date, startTime, endTime, lunchStartTime, lunchEndTime);
    }

    private static IReadOnlyList<ValidationError> Validate(
        TimeOnly startTime,
        TimeOnly endTime,
        TimeOnly? lunchStartTime,
        TimeOnly? lunchEndTime)
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

        return validationErrors;
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
