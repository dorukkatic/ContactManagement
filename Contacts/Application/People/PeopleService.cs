using Contacts.Application.Helpers;
using Contacts.Contracts.Common;
using Contacts.Contracts.ContactInfos;
using Contacts.Contracts.People;
using Contacts.DataAccess;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Contacts.Application.People;

public class PeopleService : IPeopleService
{
    private readonly ContactsDbContext db;
    private readonly IContactInfosService contactInfosService;

    public PeopleService(
        ContactsDbContext db, 
        IContactInfosService contactInfosService)
    {
        this.db = db;
        this.contactInfosService = contactInfosService;
    }

    public async Task<Guid> AddPerson(AddPersonRequest request)
    {
        var mapper = new PersonMapper();
        var person = mapper.AddPersonRequestToPerson(request);

        db.People.Add(person);
        await db.SaveChangesAsync();

        return person.Id;
    }

    public async Task<Result<PersonDetailResponse>> GetPersonById(Guid id)
    {
        var person =
            await db.People.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (person is null) return Result.Fail("Person not found");
        var contactInfos = await contactInfosService.GetContactInfos(id, 1, 10);

        var mapper = new PersonMapper();
        return mapper.PersonToPersonDetailResponse(person, contactInfos);
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
        
        if (!QueryPagingHelper.ShouldFetchData(pageNumber, pageSize, totalCount))
        {
            return new PagedResponse<PersonResponse>(
                pageNumber,
                pageSize,
                totalCount,
                []);
        }
        
        var mapper = new PersonMapper();

        var data =
            await people
                .ApplyOrdering(orderBy, isDescending)
                .ApplyPaging(pageNumber, pageSize)
                .Select(p => mapper.PersonToPersonResponse(p))
                .ToListAsync(cancellationToken);

        return new PagedResponse<PersonResponse>(pageNumber, pageSize, totalCount, data);
    }

    public async Task<Result> DeletePerson(Guid id)
    {
        var person = await db.People.FindAsync(id);
        
        if (person == null) return Result.Fail(new Error("Person not found"));
        
        db.People.Remove(person);
        await db.SaveChangesAsync();
        
        return Result.Ok();
    }
}