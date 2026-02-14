using FlexTracker.Domain;
using FlexTracker.Domain.Entities;

namespace FlexTracker.Application.TimeEntries;

public sealed record CreateTimeEntryResult(
    int Id,
    TimeEntry Entry);
