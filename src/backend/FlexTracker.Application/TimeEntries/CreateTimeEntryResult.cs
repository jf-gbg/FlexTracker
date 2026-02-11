using FlexTracker.Domain;

namespace FlexTracker.Application.TimeEntries;

public sealed record CreateTimeEntryResult(
    int Id,
    TimeEntry Entry);
