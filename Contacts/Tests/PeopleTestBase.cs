using Bogus;
using Bogus.DataSets;
using Contacts.Contracts.People;
using Contacts.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using Person = Contacts.Domain.Person;

namespace Contracts.Application.Tests.Unit;

public abstract class PeopleTestBase : IDisposable
{
    protected readonly ContactsDbContext db;
    protected readonly Faker<AddPersonRequest> AddPersonFaker;
    protected readonly Faker<Person> PersonFaker;
    protected readonly Date DateFaker = new Faker().Date;
    protected readonly FakeTimeProvider ContactsManagementTimeProvider = new();

    protected PeopleTestBase()
    {
        var options = new DbContextOptionsBuilder<ContactsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        db = new ContactsDbContext(options, ContactsManagementTimeProvider);
        db.Database.EnsureCreated();

        AddPersonFaker = new Faker<AddPersonRequest>()
            .CustomInstantiator(f =>
                new AddPersonRequest(
                    f.Name.FirstName(),
                    f.Name.LastName(),
                    f.Company.CompanyName()));

        PersonFaker = new Faker<Person>()
            .RuleFor(p => p.Id, f => Guid.NewGuid())
            .RuleFor(p => p.FirstName, f => f.Name.FirstName())
            .RuleFor(p => p.LastName, f => f.Name.LastName())
            .RuleFor(p => p.Company, f => f.Company.CompanyName());
    }

    protected virtual Task SeedDatabaseAsync() => Task.CompletedTask;

    public void Dispose()
    {
        db.Dispose();
    }
}