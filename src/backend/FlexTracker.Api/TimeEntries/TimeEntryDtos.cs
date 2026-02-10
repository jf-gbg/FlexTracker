namespace FlexTracker.Api.TimeEntries;

public sealed record CreateTimeEntryRequestDto(
    string? Date,
    string? StartTime,
    string? EndTime,
    string? LunchStartTime,
    string? LunchEndTime);

public sealed record TimeEntryWarningDto(
    string Code,
    string Message,
    IReadOnlyList<int> OverlappingEntryIds);

public sealed record CreateTimeEntryResponseDto(
    int Id,
    string Date,
    string StartTime,
    string EndTime,
    string? LunchStartTime,
    string? LunchEndTime,
    int WorkedMinutes,
    int LunchMinutes,
    IReadOnlyList<TimeEntryWarningDto> Warnings);
