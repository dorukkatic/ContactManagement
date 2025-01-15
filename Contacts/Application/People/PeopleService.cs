using Contacts.Contracts.People;
using Contacts.DataAccess;
using Contacts.Domain;

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
        var person = await db.People.FindAsync(id);
        if (person is null) return null;
        
        var mapper = new PersonMapper();
        return mapper.PersonToPersonResponse(person);
    }
}