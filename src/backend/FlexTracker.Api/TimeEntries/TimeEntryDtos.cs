namespace FlexTracker.Api.TimeEntries;

public sealed record CreateTimeEntryRequestDto(
    string? Date,
    string? StartTime,
    string? EndTime,
    string? LunchStartTime,
    string? LunchEndTime);

public sealed record CreateTimeEntryResponseDto(
    int Id,
    string Date,
    string StartTime,
    string EndTime,
    string? LunchStartTime,
    string? LunchEndTime,
    int WorkedMinutes,
    int LunchMinutes);

public sealed record ListTimeEntryResponseDto(
    int Id,
    string Date,
    string StartTime,
    string EndTime,
    string? LunchStartTime,
    string? LunchEndTime,
    int WorkedMinutes,
    int LunchMinutes);
