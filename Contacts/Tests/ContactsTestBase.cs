using Bogus;
using Bogus.DataSets;
using Contacts.Contracts.ContactInfos;
using Contacts.Contracts.People;
using Contacts.DataAccess;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using Person = Contacts.Domain.Person;

namespace Contracts.Application.Tests.Unit;

public abstract class ContactsTestBase : IDisposable
{
    protected readonly ContactsDbContext db;
    protected readonly Faker<AddPersonRequest> AddPersonFaker;
    protected readonly Faker<AddContactInfoRequest> AddContactInfoRequestFaker;
    protected readonly Faker<Person> PersonFaker;
    protected readonly Faker<AddLocationRequest> AddLocationRequestFaker;
    protected readonly Date DateFaker = new Faker().Date;
    protected readonly FakeTimeProvider ContactsManagementTimeProvider = new();
    
    protected ContactsTestBase()
    {
        
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = 
            new DbContextOptionsBuilder<ContactsDbContext>()
                .UseSqlite(connection)
                .Options;

        db = new ContactsDbContext(options, ContactsManagementTimeProvider);
        db.Database.EnsureCreated();

        AddPersonFaker = 
            new Faker<AddPersonRequest>()
                .CustomInstantiator(f =>
                    new AddPersonRequest(
                        f.Name.FirstName(),
                        f.Name.LastName(),
                        f.Company.CompanyName()));

        PersonFaker = 
            new Faker<Person>()
                .RuleFor(p => p.Id, f => Guid.NewGuid())
                .RuleFor(p => p.FirstName, f => f.Name.FirstName())
                .RuleFor(p => p.LastName, f => f.Name.LastName())
                .RuleFor(p => p.Company, f => f.Company.CompanyName());
        
        AddLocationRequestFaker =
            new Faker<AddLocationRequest>()
                .CustomInstantiator(f => new AddLocationRequest(f.Address.City()));

        AddContactInfoRequestFaker =
            new Faker<AddContactInfoRequest>()
                .CustomInstantiator(f =>
                {
                    var type = f.PickRandom<ContactInfoTypeDTO>();
                    
                    var value = type switch
                    {
                        ContactInfoTypeDTO.Email => f.Internet.Email(),
                        ContactInfoTypeDTO.Phone => f.Phone.PhoneNumber(),
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    return new AddContactInfoRequest(type, value);
                });
    }

    protected virtual Task SeedDatabaseAsync() => Task.CompletedTask;

    public void Dispose()
    {
        db.Dispose();
    }
}