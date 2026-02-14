namespace FlexTracker.Domain.Helpers;

public sealed record ValidationError(string Field, string Message);
