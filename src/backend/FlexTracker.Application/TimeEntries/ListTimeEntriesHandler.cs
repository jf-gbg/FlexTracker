using FlexTracker.Application.Contracts;

namespace FlexTracker.Application.TimeEntries;

public sealed class ListTimeEntriesHandler
{
    private readonly ITimeEntryRepository _repository;

    public ListTimeEntriesHandler(ITimeEntryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<TimeEntryListItem>> HandleAsync(CancellationToken cancellationToken)
    {
        var entries = await _repository.ListAsync(cancellationToken);
        return entries.Select(e => new TimeEntryListItem(
                e.Id,
                e.Date,
                e.StartTime,
                e.EndTime,
                e.LunchStartTime,
                e.LunchEndTime,
                e.GetWorkedMinutes(),
                e.GetLunchMinutes()))
            .ToList();
    }
}
