namespace FlexTracker.Infrastructure.Models;

public class TimeEntryDbo
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public TimeOnly? LunchStartTime { get; set; }
    public TimeOnly? LunchEndTime { get; set; }
}
