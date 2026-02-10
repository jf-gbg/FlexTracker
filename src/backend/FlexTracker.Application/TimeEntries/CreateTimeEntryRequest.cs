namespace FlexTracker.Application.TimeEntries;

public sealed record CreateTimeEntryRequest(
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    TimeOnly? LunchStartTime,
    TimeOnly? LunchEndTime);
