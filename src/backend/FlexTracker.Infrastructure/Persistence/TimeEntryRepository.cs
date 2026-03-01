using CSharpFunctionalExtensions;
using FlexTracker.Application.Common.Errors;
using FlexTracker.Application.Contracts;
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

    public async Task<Result<int, PersistenceError>> AddAsync(TimeEntry entry, CancellationToken cancellationToken)
    {
        const string overlapErrorToken = "FT_OVERLAP_TIME_ENTRY";

        var dbo = new TimeEntryDbo
        {
            Date = entry.Date,
            StartTime = entry.StartTime,
            EndTime = entry.EndTime,
            LunchStartTime = entry.LunchStartTime,
            LunchEndTime = entry.LunchEndTime
        };
        try
        {
            _dbContext.TimeEntries.Add(dbo);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (
            ContainsExceptionMessage(exception, overlapErrorToken))
        {
            return Result.Failure<int, PersistenceError>(PersistenceError.Overlap);
        }

        return Result.Success<int, PersistenceError>(dbo.Id);
    }

    public async Task<IReadOnlyList<TimeEntry>> ListAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.TimeEntries
            .AsNoTracking()
            .OrderByDescending(entry => entry.Date)
            .ThenByDescending(entry => entry.StartTime)
            .ThenByDescending(entry => entry.Id)
            .Select(entry => TimeEntry.Rehydrate(
                entry.Id,
                entry.Date,
                entry.StartTime,
                entry.EndTime,
                entry.LunchStartTime,
                entry.LunchEndTime))
            .ToListAsync(cancellationToken);
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

    private static bool ContainsExceptionMessage(Exception exception, string token)
    {
        for (var current = exception; current is not null; current = current.InnerException)
        {
            if (current.Message.Contains(token, StringComparison.Ordinal))
                return true;
        }

        return false;
    }
}
