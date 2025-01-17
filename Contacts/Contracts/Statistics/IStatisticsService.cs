using FluentResults;

namespace Contacts.Contracts.Statistics;

public interface IStatisticsService
{
    Task<Result<IEnumerable<PeopleCountByLocationResponse>>> GetPeopleCountByLocations(CancellationToken cancellationToken = default);
}