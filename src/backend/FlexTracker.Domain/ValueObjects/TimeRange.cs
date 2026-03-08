using FlexTracker.Domain.Validation;

namespace FlexTracker.Domain.ValueObjects;

public sealed record TimeRange
{
    public TimeOnly Start { get; }
    public TimeOnly End { get; }
    public int DurationMinutes => (int)(End - Start).TotalMinutes;

    private TimeRange(TimeOnly start, TimeOnly end)
    {
        Start = start;
        End = end;
    }

    public static TimeRange? Create(TimeOnly start, TimeOnly end, out IReadOnlyList<ValidationError> errors)
    {
        var validationErrors = new List<ValidationError>();
        if (end <= start)
        {
            validationErrors.Add(new ValidationError("endTime", "End time must be after start time."));
        }

        errors = validationErrors;
        if (validationErrors.Count > 0)
        {
            return null;
        }

        return new TimeRange(start, end);
    }

    public bool Contains(TimeRange innerRange) =>
        Start <= innerRange.Start && innerRange.End <= End;
}
