using System.Globalization;
using FlexTracker.Application.TimeEntries;

namespace FlexTracker.Api.TimeEntries;

public static class TimeEntryEndpoints
{
    public static IEndpointRouteBuilder MapTimeEntryEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/time-entries", async (
            CreateTimeEntryRequestDto request,
            CreateTimeEntryHandler handler,
            CancellationToken cancellationToken) =>
        {
            var errors = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);

            if (!TryParseDate(request.Date, out var date))
                AddError(errors, "date", "Date must be in yyyy-MM-dd format.");

            if (!TryParseTime(request.StartTime, out var startTime))
                AddError(errors, "startTime", "Time must be in HH:mm format.");

            if (!TryParseTime(request.EndTime, out var endTime))
                AddError(errors, "endTime", "Time must be in HH:mm format.");

            TimeOnly? lunchStartTime = null;
            if (!string.IsNullOrWhiteSpace(request.LunchStartTime))
            {
                if (TryParseTime(request.LunchStartTime, out var parsedLunchStart))
                    lunchStartTime = parsedLunchStart;
                else
                    AddError(errors, "lunchStartTime", "Time must be in HH:mm format.");
            }

            TimeOnly? lunchEndTime = null;
            if (!string.IsNullOrWhiteSpace(request.LunchEndTime))
            {
                if (TryParseTime(request.LunchEndTime, out var parsedLunchEnd))
                    lunchEndTime = parsedLunchEnd;
                else
                    AddError(errors, "lunchEndTime", "Time must be in HH:mm format.");
            }

            if (errors.Count > 0)
                return Results.ValidationProblem(errors);

            var command = new CreateTimeEntryCommand(
                date,
                startTime,
                endTime,
                lunchStartTime,
                lunchEndTime);

            var result = await handler.HandleAsync(command, cancellationToken);
            if (result.IsFailure)
            {
                var domainErrors = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
                foreach (var error in result.Error)
                    AddError(domainErrors, error.Field, error.Message);

                return Results.ValidationProblem(domainErrors);
            }

            var entryResult = result.Value;
            var entry = entryResult.Entry;
            var response = new CreateTimeEntryResponseDto(
                entryResult.Id,
                entry.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                entry.StartTime.ToString("HH:mm", CultureInfo.InvariantCulture),
                entry.EndTime.ToString("HH:mm", CultureInfo.InvariantCulture),
                entry.LunchStartTime?.ToString("HH:mm", CultureInfo.InvariantCulture),
                entry.LunchEndTime?.ToString("HH:mm", CultureInfo.InvariantCulture),
                entry.GetWorkedMinutes(),
                entry.GetLunchMinutes());

            return Results.Created($"/time-entries/{entryResult.Id}", response);
        });

        app.MapGet("/time-entries", async (
            ListTimeEntriesHandler handler,
            CancellationToken cancellationToken) =>
        {
            var entries = await handler.HandleAsync(cancellationToken);
            var response = entries.Select(entry => new ListTimeEntryResponseDto(
                    entry.Id,
                    entry.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    entry.StartTime.ToString("HH:mm", CultureInfo.InvariantCulture),
                    entry.EndTime.ToString("HH:mm", CultureInfo.InvariantCulture),
                    entry.LunchStartTime?.ToString("HH:mm", CultureInfo.InvariantCulture),
                    entry.LunchEndTime?.ToString("HH:mm", CultureInfo.InvariantCulture),
                    entry.WorkedMinutes,
                    entry.LunchMinutes))
                .ToList();

            return Results.Ok(response);
        });

        return app;
    }

    private static bool TryParseDate(string? value, out DateOnly date)
    {
        return DateOnly.TryParseExact(
            value,
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out date);
    }

    private static bool TryParseTime(string? value, out TimeOnly time)
    {
        return TimeOnly.TryParseExact(
            value,
            "HH:mm",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out time);
    }

    private static void AddError(IDictionary<string, string[]> errors, string field, string message)
    {
        if (errors.TryGetValue(field, out var existing))
        {
            errors[field] = existing.Concat(new[] { message }).ToArray();
            return;
        }

        errors[field] = new[] { message };
    }
}
