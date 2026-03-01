namespace FlexTracker.Domain.Validation;

public sealed record ValidationError(string Field, string Message);
