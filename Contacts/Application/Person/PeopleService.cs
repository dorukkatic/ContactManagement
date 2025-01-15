using Contacts.Contracts.Person;
using Contacts.DataAccess;

namespace Contacts.Application.Person;

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
}