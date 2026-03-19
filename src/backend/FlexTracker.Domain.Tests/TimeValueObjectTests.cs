using FlexTracker.Domain.ValueObjects;
using Xunit;

namespace FlexTracker.Domain.Tests;

public sealed class TimeValueObjectTests
{
    [Fact]
    public void TimeRange_Create_Rejects_EndBeforeStart()
    {
        var range = TimeRange.Create(new TimeOnly(9, 0), new TimeOnly(8, 59), out var errors);

        Assert.Null(range);
        Assert.Contains(errors, error => error.Field == "endTime");
    }

    [Fact]
    public void TimeRange_Create_Returns_DurationMinutes_WhenValid()
    {
        var range = TimeRange.Create(new TimeOnly(9, 0), new TimeOnly(17, 0), out var errors);

        Assert.Empty(errors);
        Assert.NotNull(range);
        Assert.Equal(480, range!.DurationMinutes);
    }

    [Fact]
    public void TimeRange_Contains_ReturnsTrue_WhenInnerRangeFits()
    {
        var outerRange = TimeRange.Create(new TimeOnly(9, 0), new TimeOnly(17, 0), out var outerErrors);
        var innerRange = TimeRange.Create(new TimeOnly(12, 0), new TimeOnly(12, 30), out var innerErrors);

        Assert.Empty(outerErrors);
        Assert.Empty(innerErrors);
        Assert.True(outerRange!.Contains(innerRange!));
    }
}
