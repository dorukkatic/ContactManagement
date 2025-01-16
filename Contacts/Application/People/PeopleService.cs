using Contacts.Contracts.Common;
using Contacts.Contracts.People;
using Contacts.DataAccess;
using Contacts.Domain;
using Microsoft.EntityFrameworkCore;

namespace Contacts.Application.People;

public class PeopleService : IPeopleService
{
    private readonly ContactsDbContext db;

    public PeopleService(ContactsDbContext db)
    {
        this.db = db;
    }

    public Task<Guid> AddPerson(AddPersonRequest request)
    {
        var mapper = new PersonMapper();
        var person = mapper.AddPersonRequestToPerson(request);

        db.People.Add(person);
        db.SaveChanges();

        return Task.FromResult(person.Id);
    }

    public async Task<PersonResponse?> GetPersonById(Guid id)
    {
        var person =
            await db.People.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (person is null) return null;

        var mapper = new PersonMapper();
        return mapper.PersonToPersonResponse(person);
    }

    public async Task<PagedResponse<PersonResponse>> GetPeople(
        int pageNumber,
        int pageSize,
        PeopleOrderByField orderBy,
        bool isDescending,
        CancellationToken cancellationToken)
    {
        var people = db.People.AsNoTracking();

        var totalCount = await people.LongCountAsync(cancellationToken);

        var skip = (pageNumber - 1) * pageSize;
        if (skip >= totalCount)
        {
            return new PagedResponse<PersonResponse>(
                pageNumber,
                pageSize,
                totalCount,
                []);
        }

        var data =
            await people
                .ApplyOrdering(orderBy, isDescending)
                .ApplyPaging(pageNumber, pageSize)
                .Select(p =>
                    new PersonResponse(
                        p.Id,
                        p.FirstName,
                        p.LastName,
                        p.Company,
                        p.CreatedAt,
                        p.UpdatedAt))
                .ToListAsync(cancellationToken);

        return new PagedResponse<PersonResponse>(pageNumber, pageSize, totalCount, data);
    }
}