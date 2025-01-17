using Contacts.Application.ContactInfos;
using Contacts.Contracts.ContactInfos;
using Contacts.Domain;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Riok.Mapperly.Abstractions;

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

        var contactInfoId = (await contactInfosService.AddContactInfo(person.Id, request)).Value;
        var contactInfo = await db.ContactInfos.FindAsync(contactInfoId);

        contactInfo.Should().NotBeNull();
        contactInfo.PersonId.Should().Be(person.Id);
        contactInfo.Value.Should().Be(request.Value);
        contactInfo.Type.Should().HaveSameValueAs(request.Type);
    }
    
    [Fact]
    public async Task AddContactInfo_ShouldReturnFailedResult_WhenPersonIdIsInvalid()
    {
        var invalidPersonId = Guid.NewGuid();
        var request = AddContactInfoRequestFaker.Generate();

        var addResult = await contactInfosService.AddContactInfo(invalidPersonId, request);
        
        addResult.IsFailed.Should().BeTrue();
    }
    
    [Fact]
    public async Task AddLocation_ShouldAddLocationToDatabase()
    {
        var person = db.People.First();
        var request = AddLocationRequestFaker.Generate();

        var locationId = (await contactInfosService.AddLocation(person.Id, request)).Value;
        var location = await db.ContactInfos.FindAsync(locationId);

        location.Should().NotBeNull();
        location?.PersonId.Should().Be(person.Id);
        location?.Value.Should().Be(request.Value.ToUpper());
        location?.Type.Should().Be(ContactInfoType.Location);
    }

    [Fact]
    public async Task AddLocation_ShouldReturnFailedResult_WhenPersonAlreadyHasLocation()
    {
        var person = db.People.First();
        var request = AddLocationRequestFaker.Generate();

        // Add initial location
        await contactInfosService.AddLocation(person.Id, request);

        // Try to add another location
        var addResult = await contactInfosService.AddLocation(person.Id, request);

        addResult.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task AddLocation_ShouldReturnFailedResult_WhenPersonIdIsInvalid()
    {
        var invalidPersonId = Guid.NewGuid();
        var request = AddLocationRequestFaker.Generate();

        var addResult = await contactInfosService.AddLocation(invalidPersonId, request);

        addResult.IsFailed.Should().BeTrue();
    }

    [Theory]
    [InlineData("istanbul")]
    [InlineData("ankara")]
    [InlineData("Izmir")]
    [InlineData("ANTALYA")]
    public async Task AddLocation_ShouldAlwaysAddTheValueAsUpperCase(string value)
    {
        var person = db.People.First();

        var request = new AddLocationRequest(value);
        
        var locationId = (await contactInfosService.AddLocation(person.Id, request)).Value;
        var location = await db.ContactInfos.FindAsync(locationId);
        
        location?.Value.Should().BeUpperCased(value);
    }
    
    [Fact]
    public async Task DeleteContactInfo_ShouldRemoveContactInfoFromDatabase()
    {
        var person = db.People.First();
        var request = AddContactInfoRequestFaker.Generate();
        var contactInfoId = (await contactInfosService.AddContactInfo(person.Id, request)).Value;

        var deleteResult = await contactInfosService.DeleteContactInfo(contactInfoId);
        var contactInfo = await db.ContactInfos.FindAsync(contactInfoId);

        deleteResult.IsSuccess.Should().BeTrue();
        contactInfo.Should().BeNull();
    }
    
    [Fact]
    public async Task DeleteContactInfo_ShouldReturnFailedResult_WhenContactInfoDoesNotExist()
    {
        var invalidContactInfoId = Guid.NewGuid();

        var deleteResult = await contactInfosService.DeleteContactInfo(invalidContactInfoId);

        deleteResult.IsFailed.Should().BeTrue();
    }
    
    [Fact]
    public async Task GetContactInfos_ShouldReturnPagedResponse()
    {
        var person = db.People.First();
        var request = AddContactInfoRequestFaker.Generate();
        await contactInfosService.AddContactInfo(person.Id, request);

        var pagedResponse = await contactInfosService.GetContactInfos(person.Id, 1, 10);

        pagedResponse.Should().NotBeNull();
        pagedResponse.Data.Should().HaveCount(1);
        pagedResponse.TotalCount.Should().Be(1);
    }
    
    [Fact]
    public async Task GetContactInfos_ShouldReturnEmptyPagedResponse_WhenPersonHasNoContactInfos()
    {
        var person = db.People.First();

        var pagedResponse = await contactInfosService.GetContactInfos(person.Id, 1, 10);

        pagedResponse.Should().NotBeNull();
        pagedResponse.Data.Should().BeEmpty();
        pagedResponse.TotalCount.Should().Be(0);
    }
    
    [Fact]
    public async Task GetContactInfos_ShouldReturnEmptyPagedResponse_WhenPageNumberIsOutOfRange()
    {
        var person = db.People.First();
        var request = AddContactInfoRequestFaker.Generate();
        await contactInfosService.AddContactInfo(person.Id, request);

        var pagedResponse = await contactInfosService.GetContactInfos(person.Id, 2, 10);

        pagedResponse.Should().NotBeNull();
        pagedResponse.Data.Should().BeEmpty();
        pagedResponse.TotalCount.Should().Be(1);
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