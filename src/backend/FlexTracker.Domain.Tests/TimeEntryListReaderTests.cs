using FlexTracker.Domain.TimeEntries;
using FlexTracker.Infrastructure.Models;
using FlexTracker.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FlexTracker.Domain.Tests;

public sealed class TimeEntryRepositoryTests
{
    [Fact]
    public async Task ListAsync_Returns_Deterministically_Ordered_Domain_Entries()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        await using (var setupContext = new AppDbContext(options))
        {
            await setupContext.Database.EnsureCreatedAsync();
            setupContext.TimeEntries.AddRange(
                new TimeEntryDbo
                {
                    Date = new DateOnly(2026, 3, 10),
                    StartTime = new TimeOnly(9, 0),
                    EndTime = new TimeOnly(17, 0),
                    LunchStartTime = new TimeOnly(12, 0),
                    LunchEndTime = new TimeOnly(12, 30)
                },
                new TimeEntryDbo
                {
                    Date = new DateOnly(2026, 3, 10),
                    StartTime = new TimeOnly(9, 0),
                    EndTime = new TimeOnly(11, 0)
                },
                new TimeEntryDbo
                {
                    Date = new DateOnly(2026, 3, 11),
                    StartTime = new TimeOnly(8, 30),
                    EndTime = new TimeOnly(16, 30)
                });

            await setupContext.SaveChangesAsync();
        }

        await using var queryContext = new AppDbContext(options);
        var repository = new TimeEntryRepository(queryContext);

        var result = await repository.ListAsync(CancellationToken.None);

        Assert.Collection(
            result,
            AssertEntry(3, new DateOnly(2026, 3, 11), new TimeOnly(8, 30), 480, 0),
            AssertEntry(2, new DateOnly(2026, 3, 10), new TimeOnly(9, 0), 120, 0),
            AssertEntry(1, new DateOnly(2026, 3, 10), new TimeOnly(9, 0), 450, 30));
    }

    private static Action<TimeEntry> AssertEntry(
        int id,
        DateOnly date,
        TimeOnly startTime,
        int workedMinutes,
        int lunchMinutes) =>
        entry =>
        {
            Assert.Equal(id, entry.Id);
            Assert.Equal(date, entry.Date);
            Assert.Equal(startTime, entry.StartTime);
            Assert.Equal(workedMinutes, entry.GetWorkedMinutes());
            Assert.Equal(lunchMinutes, entry.GetLunchMinutes());
        };
}
