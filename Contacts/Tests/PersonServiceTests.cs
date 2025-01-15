using Bogus;
using Contacts.Application;
using Contacts.Application.Person;
using Contacts.Contracts.Person;
using Contacts.DataAccess;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Contracts.Application.Tests.Unit;

public class PersonServiceTests
{
    private readonly ContactsDbContext db;
    private readonly PersonService personService;

    public PersonServiceTests()
    {
        var options = 
            new DbContextOptionsBuilder<ContactsDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        db = new ContactsDbContext(options);

        db.Database.EnsureCreated();

        personService = new PersonService(db);
    }

    [Theory]
    [InlineData(null, null)] 
    [InlineData(null, "Acme Corp")]
    [InlineData("Doe", null)] 
    [InlineData("Doe", "Acme Corp")] 
    public async Task AddPerson_ShouldAddPersonToDatabase(string? lastName, string? company)
    {
        var faker = new Faker();
        var request = new AddPersonRequest(
            faker.Name.FirstName(),
            lastName,              
            company                
        );

        var personId = await personService.AddPerson(request);

        var person = await db.People.FindAsync(personId);

        person.Should().NotBeNull();
        person.FirstName.Should().Be(request.FirstName);
        person.LastName.Should().Be(request.LastName);
        person.Company.Should().Be(request.Company);
    }

    [Fact]
    public async Task AddPerson_ShouldGenerateUniqueIds()
    {
        var faker = new Faker();
        var request1 = new AddPersonRequest(
            faker.Name.FirstName(),
            faker.Name.LastName(),
            faker.Company.CompanyName());

        var request2 = new AddPersonRequest(
            faker.Name.FirstName(),
            faker.Name.LastName(),
            faker.Company.CompanyName());

        var personId1 = await personService.AddPerson(request1);
        var personId2 = await personService.AddPerson(request2);

        personId1.Should().NotBe(personId2);
    }
}