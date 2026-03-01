using CSharpFunctionalExtensions;
using FlexTracker.Application.Common.Errors;
using FlexTracker.Domain.Entities;

namespace FlexTracker.Application.Contracts;

public interface ITimeEntryRepository
{
    Task<Result<int, PersistenceError>> AddAsync(TimeEntry entry, CancellationToken cancellationToken);
    Task<IReadOnlyList<TimeEntry>> ListAsync(CancellationToken cancellationToken);
    Task<bool> HasOverlapAsync(DateOnly date, TimeOnly startTime, TimeOnly endTime, CancellationToken cancellationToken);
}
