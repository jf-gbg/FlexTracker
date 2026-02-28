namespace FlexTracker.Application.TimeEntries;

public sealed record TimeEntryListItem(
    int Id,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    TimeOnly? LunchStartTime,
    TimeOnly? LunchEndTime,
    int WorkedMinutes,
    int LunchMinutes);
