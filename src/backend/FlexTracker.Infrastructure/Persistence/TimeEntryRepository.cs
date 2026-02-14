using FlexTracker.Application.TimeEntries;
using FlexTracker.Domain.Entities;
using FlexTracker.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace FlexTracker.Infrastructure.Persistence;

public sealed class TimeEntryRepository : ITimeEntryRepository
{
    private readonly AppDbContext _dbContext;

    public TimeEntryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> AddAsync(TimeEntry entry, CancellationToken cancellationToken)
    {
        var dbo = new TimeEntryDbo
        {
            Date = entry.Date,
            StartTime = entry.StartTime,
            EndTime = entry.EndTime,
            LunchStartTime = entry.LunchStartTime,
            LunchEndTime = entry.LunchEndTime
        };

        _dbContext.TimeEntries.Add(dbo);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return dbo.Id;
    }

    public async Task<bool> HasOverlapAsync(
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        CancellationToken cancellationToken)
    {
        return await _dbContext.TimeEntries
            .Where(entry => entry.Date == date && startTime < entry.EndTime && endTime > entry.StartTime)
            .AnyAsync(cancellationToken);
    }
}
