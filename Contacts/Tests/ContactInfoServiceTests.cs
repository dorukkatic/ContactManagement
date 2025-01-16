using Contacts.Application.ContactInfos;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Contracts.Application.Tests.Unit;

public class ContactInfoServiceTests : ContactsTestBase, IAsyncLifetime
{
    private readonly ContactInfosService contactInfosService;
    private const int PersonCount = 10;

    public ContactInfoServiceTests()
    {
        contactInfosService = new ContactInfosService(db);
    }
    
    [Fact]
    public async Task AddContactInfo_ShouldAddContactInfoToDatabase()
    {
        var person = db.People.First();
        var request = AddContactInfoRequestFaker.Generate();

        var contactInfoId = await contactInfosService.AddContactInfo(person.Id, request);
        var contactInfo = await db.ContactInfos.FindAsync(contactInfoId);

        contactInfo.Should().NotBeNull();
        contactInfo.PersonId.Should().Be(person.Id);
        contactInfo.Value.Should().Be(request.Value);
        contactInfo.Type.Should().HaveSameValueAs(request.Type);
    }
    
    [Fact]
    public async Task AddContactInfo_ShouldThrowException_WhenPersonIdIsInvalid()
    {
        var invalidPersonId = Guid.NewGuid();
        var request = AddContactInfoRequestFaker.Generate();

        Func<Task> act = async () => await contactInfosService.AddContactInfo(invalidPersonId, request);

        await act.Should().ThrowAsync<DbUpdateException>();
    }
    
    protected override async Task SeedDatabaseAsync()
    {
        var people = PersonFaker.Generate(PersonCount);
        db.People.AddRange(people);
        await db.SaveChangesAsync();
    }
    
    public async Task InitializeAsync()
    {
        await SeedDatabaseAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}