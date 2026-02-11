using CSharpFunctionalExtensions;
using FlexTracker.Domain;

namespace FlexTracker.Application.TimeEntries;

public sealed class CreateTimeEntryHandler
{
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
        {
            return Result.Failure<CreateTimeEntryResult, IReadOnlyList<ValidationError>>(errors);
        }

        var hasOverlap = await _repository.HasOverlapAsync(
            command.Date,
            command.StartTime,
            command.EndTime,
            cancellationToken);

        if (TimeEntry.HasOverlapValidationError(hasOverlap, out var overlapError))
        {
            return Result.Failure<CreateTimeEntryResult, IReadOnlyList<ValidationError>>(
                new[] { overlapError });
        }

        var id = await _repository.AddAsync(entry, cancellationToken);
        var result = new CreateTimeEntryResult(id, entry);

        return Result.Success<CreateTimeEntryResult, IReadOnlyList<ValidationError>>(result);
    }
}
