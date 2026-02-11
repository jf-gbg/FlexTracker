using FlexTracker.Domain;

namespace FlexTracker.Application.TimeEntries;

public interface ITimeEntryRepository
{
    Task<int> AddAsync(TimeEntry entry, CancellationToken cancellationToken);

    Task<bool> HasOverlapAsync(
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        CancellationToken cancellationToken);
}
