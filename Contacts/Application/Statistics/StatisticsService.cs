using Contacts.Contracts.Statistics;
using Contacts.DataAccess;
using Contacts.Domain;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Contacts.Application.Statistics;

public class StatisticsService : IStatisticsService
{
    private readonly ContactsDbContext db;

    public StatisticsService(ContactsDbContext db)
    {
        this.db = db;
    }

    public async Task<Result<IEnumerable<PeopleCountByLocationResponse>>> GetPeopleCountByLocations(
        CancellationToken cancellationToken = default)
    {
        var peopleCountByLocation = 
            await db.ContactInfos
                .Where(ci => ci.Type == ContactInfoType.Location)
                .Select(ci => new { ci.Value, ci.PersonId })
                .GroupBy(ci => ci.Value.ToUpper())
                .Select(g => 
                    new PeopleCountByLocationResponse(
                        g.Key, 
                        g.Select(l => l.PersonId).Distinct().Count()))
                .ToListAsync(cancellationToken);
        
        return peopleCountByLocation;
    }
}