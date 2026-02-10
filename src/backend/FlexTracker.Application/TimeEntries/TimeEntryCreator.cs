using FlexTracker.Domain;

namespace FlexTracker.Application.TimeEntries;

public sealed class TimeEntryCreator
{
    private readonly ITimeEntryRepository _repository;

    public TimeEntryCreator(ITimeEntryRepository repository)
    {
        _repository = repository;
    }

    public async Task<CreateTimeEntryOutcome> CreateAsync(
        CreateTimeEntryRequest request,
        CancellationToken cancellationToken)
    {
        var entry = TimeEntry.Create(
            request.Date,
            request.StartTime,
            request.EndTime,
            request.LunchStartTime,
            request.LunchEndTime,
            out var errors);

        if (entry is null)
        {
            return CreateTimeEntryOutcome.Failure(errors);
        }

        var overlaps = await _repository.FindOverlapsAsync(
            request.Date,
            request.StartTime,
            request.EndTime,
            cancellationToken);

        var warnings = overlaps.Count == 0
            ? Array.Empty<TimeEntryWarning>()
            : new[]
            {
                new TimeEntryWarning(
                    "overlap",
                    "Time entry overlaps existing entries.",
                    overlaps.Select(overlap => overlap.Id).ToArray())
            };

        var id = await _repository.AddAsync(entry, cancellationToken);
        var result = new CreateTimeEntryResult(id, entry, warnings);

        return CreateTimeEntryOutcome.Success(result);
    }
}
