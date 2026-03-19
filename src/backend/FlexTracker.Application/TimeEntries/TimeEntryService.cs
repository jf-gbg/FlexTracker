using FlexTracker.Domain.TimeEntries;
using FlexTracker.Domain.Validation;

namespace FlexTracker.Application.TimeEntries;

public sealed class TimeEntryService
{
    private static readonly ValidationError OverlapError = new(
        "timeRange",
        "Time entry overlaps an existing entry.");

    private readonly ITimeEntryRepository _repository;

    public TimeEntryService(ITimeEntryRepository repository)
    {
        _repository = repository;
    }

    public async Task<(TimeEntry? Entry, IReadOnlyList<ValidationError> Errors)> CreateAsync(
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        TimeOnly? lunchStartTime,
        TimeOnly? lunchEndTime,
        CancellationToken cancellationToken)
    {
        var entry = TimeEntry.Create(
            date,
            startTime,
            endTime,
            lunchStartTime,
            lunchEndTime,
            out var errors);

        if (entry is null)
            return (null, errors);

        var hasOverlap = await _repository.HasOverlapAsync(date, startTime, endTime, cancellationToken);
        if (hasOverlap)
            return (null, [OverlapError]);

        var id = await _repository.AddAsync(entry, cancellationToken);
        if (!id.HasValue)
            return (null, [OverlapError]);

        return (
            TimeEntry.Rehydrate(
                id.Value,
                entry.Date,
                entry.StartTime,
                entry.EndTime,
                entry.LunchStartTime,
                entry.LunchEndTime),
            Array.Empty<ValidationError>());
    }

    public Task<IReadOnlyList<TimeEntry>> ListAsync(CancellationToken cancellationToken) =>
        _repository.ListAsync(cancellationToken);
}