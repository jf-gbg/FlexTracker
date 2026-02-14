namespace FlexTracker.Application.TimeEntries;

public sealed record CreateTimeEntryCommand(
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    TimeOnly? LunchStartTime,
    TimeOnly? LunchEndTime);
