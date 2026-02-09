using FlexTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Default");
    EnsureSqliteDirectoryExists(connectionString);
    options.UseSqlite(connectionString);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();

static void EnsureSqliteDirectoryExists(string? connectionString)
{
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        return;
    }

    const string prefix = "Data Source=";
    var start = connectionString.IndexOf(prefix, StringComparison.OrdinalIgnoreCase);
    if (start < 0)
    {
        return;
    }

    var path = connectionString[(start + prefix.Length)..].Trim();
    if (path.Length == 0 || path.Contains(":", StringComparison.Ordinal) && Path.IsPathRooted(path))
    {
        // Absolute path or empty; still attempt to ensure directory if applicable.
    }

    var directory = Path.GetDirectoryName(path);
    if (string.IsNullOrWhiteSpace(directory))
    {
        return;
    }

    Directory.CreateDirectory(directory);
}
