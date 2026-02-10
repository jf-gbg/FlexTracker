using System.Globalization;
using FlexTracker.Api.TimeEntries;
using FlexTracker.Application.TimeEntries;
using FlexTracker.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Default");
    CheckSqLiteDirectory(connectionString);
    options.UseSqlite(connectionString);
});
builder.Services.AddScoped<ITimeEntryRepository, TimeEntryRepository>();
builder.Services.AddScoped<TimeEntryCreator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/time-entries", async (
    CreateTimeEntryRequestDto request,
    TimeEntryCreator creator,
    CancellationToken cancellationToken) =>
{
    var errors = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);

    if (!TryParseDate(request.Date, out var date))
    {
        AddError(errors, "date", "Date must be in yyyy-MM-dd format.");
    }

    if (!TryParseTime(request.StartTime, out var startTime))
    {
        AddError(errors, "startTime", "Time must be in HH:mm format.");
    }

    if (!TryParseTime(request.EndTime, out var endTime))
    {
        AddError(errors, "endTime", "Time must be in HH:mm format.");
    }

    TimeOnly? lunchStartTime = null;
    if (!string.IsNullOrWhiteSpace(request.LunchStartTime))
    {
        if (TryParseTime(request.LunchStartTime, out var parsedLunchStart))
        {
            lunchStartTime = parsedLunchStart;
        }
        else
        {
            AddError(errors, "lunchStartTime", "Time must be in HH:mm format.");
        }
    }

    TimeOnly? lunchEndTime = null;
    if (!string.IsNullOrWhiteSpace(request.LunchEndTime))
    {
        if (TryParseTime(request.LunchEndTime, out var parsedLunchEnd))
        {
            lunchEndTime = parsedLunchEnd;
        }
        else
        {
            AddError(errors, "lunchEndTime", "Time must be in HH:mm format.");
        }
    }

    if (errors.Count > 0)
    {
        return Results.ValidationProblem(errors);
    }

    var createRequest = new CreateTimeEntryRequest(
        date,
        startTime,
        endTime,
        lunchStartTime,
        lunchEndTime);

    var outcome = await creator.CreateAsync(createRequest, cancellationToken);
    if (!outcome.IsSuccess || outcome.Result is null)
    {
        var domainErrors = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
        foreach (var error in outcome.Errors)
        {
            AddError(domainErrors, error.Field, error.Message);
        }

        return Results.ValidationProblem(domainErrors);
    }

    var result = outcome.Result;
    var entry = result.Entry;
    var response = new CreateTimeEntryResponseDto(
        result.Id,
        entry.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
        entry.StartTime.ToString("HH:mm", CultureInfo.InvariantCulture),
        entry.EndTime.ToString("HH:mm", CultureInfo.InvariantCulture),
        entry.LunchStartTime?.ToString("HH:mm", CultureInfo.InvariantCulture),
        entry.LunchEndTime?.ToString("HH:mm", CultureInfo.InvariantCulture),
        entry.GetWorkedMinutes(),
        entry.GetLunchMinutes(),
        result.Warnings
            .Select(warning => new TimeEntryWarningDto(
                warning.Code,
                warning.Message,
                warning.OverlappingEntryIds))
            .ToArray());

    return Results.Created($"/time-entries/{result.Id}", response);
});

app.Run();

static void CheckSqLiteDirectory(string? connectionString)
{
    if (string.IsNullOrWhiteSpace(connectionString))
        return;

    var builder = new SqliteConnectionStringBuilder(connectionString);
    var dataSource = builder.DataSource;
    if (string.IsNullOrWhiteSpace(dataSource))
        return;

    var directory = Path.GetDirectoryName(dataSource);
    if (string.IsNullOrWhiteSpace(directory))
        return;

    Directory.CreateDirectory(directory);
}

static bool TryParseDate(string? value, out DateOnly date)
{
    return DateOnly.TryParseExact(
        value,
        "yyyy-MM-dd",
        CultureInfo.InvariantCulture,
        DateTimeStyles.None,
        out date);
}

static bool TryParseTime(string? value, out TimeOnly time)
{
    return TimeOnly.TryParseExact(
        value,
        "HH:mm",
        CultureInfo.InvariantCulture,
        DateTimeStyles.None,
        out time);
}

static void AddError(IDictionary<string, string[]> errors, string field, string message)
{
    if (errors.TryGetValue(field, out var existing))
    {
        errors[field] = existing.Concat(new[] { message }).ToArray();
        return;
    }

    errors[field] = new[] { message };
}
