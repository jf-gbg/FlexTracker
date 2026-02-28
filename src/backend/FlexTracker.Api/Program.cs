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
builder.Services.AddScoped<CreateTimeEntryHandler>();
builder.Services.AddScoped<ListTimeEntriesHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapTimeEntryEndpoints();

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

