using FlexTracker.Domain.Entities;
using Xunit;

namespace FlexTracker.Domain.Tests;

public sealed class TimeEntryTests
{
    [Fact]
    public void Create_Rejects_EndBeforeStart()
    {
        var entry = TimeEntry.Create(
            new DateOnly(2026, 2, 10),
            new TimeOnly(9, 0),
            new TimeOnly(8, 59),
            null,
            null,
            out var errors);

        Assert.Null(entry);
        Assert.Contains(errors, error => error.Field == "endTime");
    }

    [Fact]
    public void Create_Rejects_LunchMissingPair()
    {
        var entry = TimeEntry.Create(
            new DateOnly(2026, 2, 10),
            new TimeOnly(9, 0),
            new TimeOnly(17, 0),
            new TimeOnly(12, 0),
            null,
            out var errors);

        Assert.Null(entry);
        Assert.Contains(errors, error => error.Field == "lunchStartTime");
        Assert.Contains(errors, error => error.Field == "lunchEndTime");
    }

    [Fact]
    public void Create_Rejects_LunchEndBeforeStart()
    {
        var entry = TimeEntry.Create(
            new DateOnly(2026, 2, 10),
            new TimeOnly(9, 0),
            new TimeOnly(17, 0),
            new TimeOnly(13, 0),
            new TimeOnly(12, 30),
            out var errors);

        Assert.Null(entry);
        Assert.Contains(errors, error => error.Field == "lunchEndTime");
    }

    [Fact]
    public void Create_Rejects_LunchOutsideInterval()
    {
        var entry = TimeEntry.Create(
            new DateOnly(2026, 2, 10),
            new TimeOnly(9, 0),
            new TimeOnly(17, 0),
            new TimeOnly(8, 30),
            new TimeOnly(9, 15),
            out var errors);

        Assert.Null(entry);
        Assert.Contains(errors, error => error.Field == "lunchStartTime");
        Assert.Contains(errors, error => error.Field == "lunchEndTime");
    }

    [Fact]
    public void GetLunchMinutes_ReturnsZero_WhenMissing()
    {
        var entry = TimeEntry.Create(
            new DateOnly(2026, 2, 10),
            new TimeOnly(9, 0),
            new TimeOnly(17, 0),
            null,
            null,
            out var errors);

        Assert.Empty(errors);
        Assert.NotNull(entry);
        Assert.Equal(0, entry!.GetLunchMinutes());
    }

    [Fact]
    public void GetWorkedMinutes_NoLunch()
    {
        var entry = TimeEntry.Create(
            new DateOnly(2026, 2, 10),
            new TimeOnly(9, 0),
            new TimeOnly(17, 0),
            null,
            null,
            out var errors);

        Assert.Empty(errors);
        Assert.NotNull(entry);
        Assert.Equal(480, entry!.GetWorkedMinutes());
    }

    [Fact]
    public void GetWorkedMinutes_WithLunch()
    {
        var entry = TimeEntry.Create(
            new DateOnly(2026, 2, 10),
            new TimeOnly(9, 0),
            new TimeOnly(17, 0),
            new TimeOnly(12, 0),
            new TimeOnly(12, 30),
            out var errors);

        Assert.Empty(errors);
        Assert.NotNull(entry);
        Assert.Equal(30, entry!.GetLunchMinutes());
        Assert.Equal(450, entry.GetWorkedMinutes());
    }

    [Fact]
    public void HasOverlapValidationError_ReturnsError_WhenOverlapExists()
    {
        var hasError = TimeEntry.HasOverlapValidationError(true, out var error);

        Assert.True(hasError);
        Assert.Equal("timeRange", error.Field);
    }
}
