namespace FlexTracker.Api.TimeEntries;

public sealed record CreateTimeEntryRequestDto(
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    TimeOnly? LunchStartTime,
    TimeOnly? LunchEndTime);

public sealed record TimeEntryDto(
    int Id,
    string Date,
    string StartTime,
    string EndTime,
    string? LunchStartTime,
    string? LunchEndTime,
    int WorkedMinutes,
    int LunchMinutes);
