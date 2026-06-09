using System.Globalization;
using FlexTracker.Application.TimeEntries;

namespace FlexTracker.Api.TimeEntries;

public static class TimeEntryEndpoints
{
    private static readonly string TidFormat = "HH:mm";
    
    public static IEndpointRouteBuilder MapTimeEntryEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/time-entries", async (
            CreateTimeEntryRequestDto request,
            TimeEntryService service,
            CancellationToken cancellationToken) =>
        {
            var (entry, errors) = await service.CreateAsync(
                request.Date,
                request.StartTime,
                request.EndTime,
                request.LunchStartTime,
                request.LunchEndTime,
                cancellationToken);

            if (entry is null)
            {
                var domainErrors = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
                foreach (var error in errors)
                    AddError(domainErrors, error.Field, error.Message);

                return Results.ValidationProblem(domainErrors);
            }
            
            var response = new TimeEntryDto(
                entry.Id,
                entry.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                entry.StartTime.ToString(TidFormat, CultureInfo.InvariantCulture),
                entry.EndTime.ToString(TidFormat, CultureInfo.InvariantCulture),
                entry.LunchStartTime?.ToString(TidFormat, CultureInfo.InvariantCulture),
                entry.LunchEndTime?.ToString(TidFormat, CultureInfo.InvariantCulture),
                entry.WorkedMinutes,
                entry.LunchMinutes);

            return Results.Created($"/time-entries/{entry.Id}", response);
        });

        app.MapGet("/time-entries", async (
            TimeEntryService service,
            CancellationToken cancellationToken) =>
        {
            var entries = await service.ListAsync(cancellationToken);
            var response = entries.Select(entry => new TimeEntryDto(
                    entry.Id,
                    entry.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    entry.StartTime.ToString(TidFormat, CultureInfo.InvariantCulture),
                    entry.EndTime.ToString(TidFormat, CultureInfo.InvariantCulture),
                    entry.LunchStartTime?.ToString(TidFormat, CultureInfo.InvariantCulture),
                    entry.LunchEndTime?.ToString(TidFormat, CultureInfo.InvariantCulture),
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
