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
    public void UpdateTimes_Rejects_WhenExistingLunchFallsOutsideNewInterval()
    {
        var entry = TimeEntry.Create(
            new DateOnly(2026, 2, 10),
            new TimeOnly(9, 0),
            new TimeOnly(17, 0),
            new TimeOnly(12, 0),
            new TimeOnly(12, 30),
            out var createErrors);

        Assert.Empty(createErrors);
        Assert.NotNull(entry);

        var errors = entry!.UpdateStartAndEndTimes(new TimeOnly(13, 0), new TimeOnly(17, 0));

        Assert.Contains(errors, error => error.Field == "lunchStartTime");
        Assert.Equal(new TimeOnly(9, 0), entry.StartTime);
        Assert.Equal(new TimeOnly(17, 0), entry.EndTime);
    }

    [Fact]
    public void UpdateLunchBreak_UpdatesAggregateState_WhenIntervalIsValid()
    {
        var entry = TimeEntry.Create(
            new DateOnly(2026, 2, 10),
            new TimeOnly(9, 0),
            new TimeOnly(17, 0),
            null,
            null,
            out var createErrors);

        Assert.Empty(createErrors);
        Assert.NotNull(entry);

        var errors = entry!.UpdateLunchBreak(new TimeOnly(12, 0), new TimeOnly(12, 30));

        Assert.Empty(errors);
        Assert.Equal(new TimeOnly(12, 0), entry.LunchStartTime);
        Assert.Equal(new TimeOnly(12, 30), entry.LunchEndTime);
        Assert.Equal(30, entry.GetLunchMinutes());
        Assert.Equal(450, entry.GetWorkedMinutes());
    }

    [Fact]
    public void RemoveLunchBreak_ClearsLunch_AndRecomputesWorkedMinutes()
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

        entry!.RemoveLunchBreak();

        Assert.Null(entry.LunchStartTime);
        Assert.Null(entry.LunchEndTime);
        Assert.Equal(0, entry.GetLunchMinutes());
        Assert.Equal(480, entry.GetWorkedMinutes());
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
    public void Rehydrate_Sets_Id_And_ComputedValues()
    {
        var entry = TimeEntry.Rehydrate(
            42,
            new DateOnly(2026, 2, 10),
            new TimeOnly(9, 0),
            new TimeOnly(17, 0),
            new TimeOnly(12, 0),
            new TimeOnly(12, 30));

        Assert.Equal(42, entry.Id);
        Assert.Equal(30, entry.GetLunchMinutes());
        Assert.Equal(450, entry.GetWorkedMinutes());
    }
}

