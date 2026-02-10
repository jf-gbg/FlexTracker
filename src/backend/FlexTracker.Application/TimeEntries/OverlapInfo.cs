namespace FlexTracker.Application.TimeEntries;

public sealed record OverlapInfo(int Id, TimeOnly StartTime, TimeOnly EndTime);
