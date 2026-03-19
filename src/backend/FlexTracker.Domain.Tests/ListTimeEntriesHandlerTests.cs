using FlexTracker.Application.TimeEntries;
using FlexTracker.Domain.TimeEntries;
using Xunit;

namespace FlexTracker.Domain.Tests;

public sealed class TimeEntryServiceTests
{
    [Fact]
    public async Task ListAsync_Returns_Items_From_Repository()
    {
        var expected = new[]
        {
            TimeEntry.Rehydrate(
                7,
                new DateOnly(2026, 3, 10),
                new TimeOnly(9, 0),
                new TimeOnly(17, 0),
                new TimeOnly(12, 0),
                new TimeOnly(12, 30))
        };

        var service = new TimeEntryService(new FakeTimeEntryRepository
        {
            OnListAsync = cancellationToken => Task.FromResult<IReadOnlyList<TimeEntry>>(expected)
        });

        var result = await service.ListAsync(CancellationToken.None);

        Assert.Same(expected, result);
    }

    [Fact]
    public async Task CreateAsync_Returns_ValidationErrors_From_Domain()
    {
        var service = new TimeEntryService(new FakeTimeEntryRepository());

        var (entry, errors) = await service.CreateAsync(
            new DateOnly(2026, 3, 10),
            new TimeOnly(17, 0),
            new TimeOnly(9, 0),
            null,
            null,
            CancellationToken.None);

        Assert.Null(entry);
        Assert.Contains(errors, error => error.Field == "endTime");
    }

    [Fact]
    public async Task CreateAsync_Returns_OverlapError_When_Repository_Finds_Overlap()
    {
        var service = new TimeEntryService(new FakeTimeEntryRepository
        {
            OnHasOverlapAsync = (date, startTime, endTime, cancellationToken) => Task.FromResult(true)
        });

        var (entry, errors) = await service.CreateAsync(
            new DateOnly(2026, 3, 10),
            new TimeOnly(9, 0),
            new TimeOnly(17, 0),
            null,
            null,
            CancellationToken.None);

        Assert.Null(entry);
        Assert.Contains(errors, error => error.Field == "timeRange");
    }

    [Fact]
    public async Task CreateAsync_Returns_Persisted_Entry_When_Successful()
    {
        var service = new TimeEntryService(new FakeTimeEntryRepository
        {
            OnHasOverlapAsync = (date, startTime, endTime, cancellationToken) => Task.FromResult(false),
            OnAddAsync = (entry, cancellationToken) => Task.FromResult<int?>(42)
        });

        var (entry, errors) = await service.CreateAsync(
            new DateOnly(2026, 3, 10),
            new TimeOnly(9, 0),
            new TimeOnly(17, 0),
            new TimeOnly(12, 0),
            new TimeOnly(12, 30),
            CancellationToken.None);

        Assert.Empty(errors);
        Assert.NotNull(entry);
        Assert.Equal(42, entry!.Id);
        Assert.Equal(450, entry.GetWorkedMinutes());
        Assert.Equal(30, entry.GetLunchMinutes());
    }

    private sealed class FakeTimeEntryRepository : ITimeEntryRepository
    {
        public Func<TimeEntry, CancellationToken, Task<int?>> OnAddAsync { get; init; } =
            (entry, cancellationToken) => throw new NotSupportedException();

        public Func<DateOnly, TimeOnly, TimeOnly, CancellationToken, Task<bool>> OnHasOverlapAsync { get; init; } =
            (date, startTime, endTime, cancellationToken) => throw new NotSupportedException();

        public Task<int?> AddAsync(TimeEntry entry, CancellationToken cancellationToken) =>
            OnAddAsync(entry, cancellationToken);

        public Task<bool> HasOverlapAsync(
            DateOnly date,
            TimeOnly startTime,
            TimeOnly endTime,
            CancellationToken cancellationToken) =>
            OnHasOverlapAsync(date, startTime, endTime, cancellationToken);

        public Func<CancellationToken, Task<IReadOnlyList<TimeEntry>>> OnListAsync { get; init; } =
            cancellationToken => throw new NotSupportedException();

        public Task<IReadOnlyList<TimeEntry>> ListAsync(CancellationToken cancellationToken) =>
            OnListAsync(cancellationToken);
    }
}
