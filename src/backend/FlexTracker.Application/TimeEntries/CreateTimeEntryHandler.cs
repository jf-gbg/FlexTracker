using CSharpFunctionalExtensions;
using FlexTracker.Application.Common.Errors;
using FlexTracker.Application.Contracts;
using FlexTracker.Domain.Entities;
using FlexTracker.Domain.Validation;

namespace FlexTracker.Application.TimeEntries;

public sealed class CreateTimeEntryHandler
{
    private static readonly ValidationError OverlapError = new(
        "timeRange",
        "Time entry overlaps an existing entry.");

    private readonly ITimeEntryRepository _repository;

    public CreateTimeEntryHandler(ITimeEntryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<CreateTimeEntryResult, IReadOnlyList<ValidationError>>> HandleAsync(
        CreateTimeEntryCommand command,
        CancellationToken cancellationToken)
    {
        var entry = TimeEntry.Create(
            command.Date,
            command.StartTime,
            command.EndTime,
            command.LunchStartTime,
            command.LunchEndTime,
            out var errors);

        if (entry is null)
            return Result.Failure<CreateTimeEntryResult, IReadOnlyList<ValidationError>>(errors);

        var hasOverlap = await _repository.HasOverlapAsync(
            command.Date,
            command.StartTime,
            command.EndTime,
            cancellationToken);

        if (hasOverlap)
            return Result.Failure<CreateTimeEntryResult, IReadOnlyList<ValidationError>>(
                new[] { OverlapError });

        var addResult = await _repository.AddAsync(entry, cancellationToken);
        if (addResult.IsFailure && addResult.Error == PersistenceError.Overlap)
            return Result.Failure<CreateTimeEntryResult, IReadOnlyList<ValidationError>>(
                new[] { OverlapError });

        var id = addResult.Value;
        var result = new CreateTimeEntryResult(id, entry);

        return Result.Success<CreateTimeEntryResult, IReadOnlyList<ValidationError>>(result);
    }
}
