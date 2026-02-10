using FlexTracker.Domain;

namespace FlexTracker.Application.TimeEntries;

public sealed class CreateTimeEntryOutcome
{
    private CreateTimeEntryOutcome(
        CreateTimeEntryResult? result,
        IReadOnlyList<ValidationError> errors)
    {
        Result = result;
        Errors = errors;
    }

    public CreateTimeEntryResult? Result { get; }
    public IReadOnlyList<ValidationError> Errors { get; }

    public bool IsSuccess => Result is not null;

    public static CreateTimeEntryOutcome Success(CreateTimeEntryResult result)
    {
        return new CreateTimeEntryOutcome(result, Array.Empty<ValidationError>());
    }

    public static CreateTimeEntryOutcome Failure(IReadOnlyList<ValidationError> errors)
    {
        return new CreateTimeEntryOutcome(null, errors);
    }
}
