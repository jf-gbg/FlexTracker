namespace FlexTracker.Domain;

public sealed record ValidationError(string Field, string Message);
