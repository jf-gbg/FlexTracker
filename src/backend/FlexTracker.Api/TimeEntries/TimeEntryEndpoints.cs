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
            var command = new CreateTimeEntryCommand(
                request.Date,
                request.StartTime,
                request.EndTime,
                request.LunchStartTime,
                request.LunchEndTime);

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
            var response = new TimeEntryDto(
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
            var response = entries.Select(entry => new TimeEntryDto(
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
