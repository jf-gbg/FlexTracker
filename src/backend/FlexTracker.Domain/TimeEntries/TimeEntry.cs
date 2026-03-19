using FlexTracker.Domain.Validation;
using FlexTracker.Domain.ValueObjects;

namespace FlexTracker.Domain.TimeEntries;

public sealed class TimeEntry
{
    private const string LunchWithinWorkIntervalMessage = "Lunch must be within work interval.";

    public int Id { get; }
    public DateOnly Date { get; }
    public TimeRange WorkTime { get; private set; }
    public TimeRange? LunchBreakTime { get; private set; }

    public TimeOnly StartTime => WorkTime.Start;
    public TimeOnly EndTime => WorkTime.End;
    public TimeOnly? LunchStartTime => LunchBreakTime?.Start;
    public TimeOnly? LunchEndTime => LunchBreakTime?.End;

    private TimeEntry(
        int id,
        DateOnly date,
        TimeRange workTime,
        TimeRange? lunchBreakTime)
    {
        Id = id;
        Date = date;
        WorkTime = workTime;
        LunchBreakTime = lunchBreakTime;
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

        var hasLunchStart = lunchStartTime.HasValue;
        var hasLunchEnd = lunchEndTime.HasValue;
        if (hasLunchStart != hasLunchEnd)
        {
            validationErrors.Add(new ValidationError("lunchStartTime", "Lunch start and end must both be provided."));
            validationErrors.Add(new ValidationError("lunchEndTime", "Lunch start and end must both be provided."));
        }

        var workTime = TimeRange.Create(startTime, endTime, out var workTimeErrors);
        validationErrors.AddRange(workTimeErrors);

        TimeRange? lunchBreakTime = null;
        if (hasLunchStart && hasLunchEnd)
        {
            lunchBreakTime = TimeRange.Create(lunchStartTime!.Value, lunchEndTime!.Value, out var lunchErrors);
            if (lunchErrors.Count > 0)
            {
                validationErrors.Add(new ValidationError("lunchEndTime", "Lunch end must be after lunch start."));
            }
        }

        if (workTime is not null && lunchBreakTime is not null && !workTime.Contains(lunchBreakTime))
        {
            AddLunchOutsideWorkIntervalErrors(validationErrors);
        }

        errors = validationErrors;
        if (validationErrors.Count > 0)
        {
            return null;
        }

        return new TimeEntry(0, date, workTime!, lunchBreakTime);
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
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than zero.");
        }

        var entry = Create(date, startTime, endTime, lunchStartTime, lunchEndTime, out _);
        if (entry is null)
        {
            throw new InvalidOperationException("Cannot rehydrate invalid time entry data.");
        }

        return new TimeEntry(id, entry.Date, entry.WorkTime, entry.LunchBreakTime);
    }

    public IReadOnlyList<ValidationError> UpdateStartAndEndTimes(TimeOnly startTime, TimeOnly endTime)
    {
        var validationErrors = new List<ValidationError>();

        var workTime = TimeRange.Create(startTime, endTime, out var workTimeErrors);
        validationErrors.AddRange(workTimeErrors);

        if (workTime is not null && LunchBreakTime is not null && !workTime.Contains(LunchBreakTime))
        {
            AddLunchOutsideWorkIntervalErrors(validationErrors);
        }

        if (validationErrors.Count > 0)
        {
            return validationErrors;
        }

        WorkTime = workTime!;
        return Array.Empty<ValidationError>();
    }

    public IReadOnlyList<ValidationError> UpdateLunchBreak(TimeOnly lunchStartTime, TimeOnly lunchEndTime)
    {
        var validationErrors = new List<ValidationError>();

        var lunchBreakTime = TimeRange.Create(lunchStartTime, lunchEndTime, out var lunchErrors);
        if (lunchErrors.Count > 0)
        {
            validationErrors.Add(new ValidationError("lunchEndTime", "Lunch end must be after lunch start."));
        }

        if (lunchBreakTime is not null && !WorkTime.Contains(lunchBreakTime))
        {
            AddLunchOutsideWorkIntervalErrors(validationErrors);
        }

        if (validationErrors.Count > 0)
        {
            return validationErrors;
        }

        LunchBreakTime = lunchBreakTime;
        return Array.Empty<ValidationError>();
    }

    public void RemoveLunchBreak()
    {
        LunchBreakTime = null;
    }

    public int GetLunchMinutes() => LunchBreakTime?.DurationMinutes ?? 0;

    public int GetWorkedMinutes() => WorkTime.DurationMinutes - GetLunchMinutes();

    private static void AddLunchOutsideWorkIntervalErrors(ICollection<ValidationError> validationErrors)
    {
        validationErrors.Add(new ValidationError("lunchStartTime", LunchWithinWorkIntervalMessage));
        validationErrors.Add(new ValidationError("lunchEndTime", LunchWithinWorkIntervalMessage));
    }
}
