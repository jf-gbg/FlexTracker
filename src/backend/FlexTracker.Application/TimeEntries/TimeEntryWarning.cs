namespace FlexTracker.Application.TimeEntries;

public sealed record TimeEntryWarning(
    string Code,
    string Message,
    IReadOnlyList<int> OverlappingEntryIds);
