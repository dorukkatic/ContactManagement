﻿using Bogus;
using Contacts.Application.People;
using FluentAssertions;
using FluentAssertions.Common;

namespace Contracts.Application.Tests.Unit;

public class PeopleServiceTests : PeopleTestBase, IAsyncLifetime
{
    private const int PeopleCount = 10;

    private readonly PeopleService peopleService;

    public PeopleServiceTests()
    {
        peopleService = new PeopleService(db);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(null, "Acme Corp")]
    [InlineData("Doe", null)]
    [InlineData("Doe", "Acme Corp")]
    public async Task AddPerson_ShouldAddPersonToDatabase(string? lastName, string? company)
    {
        var fakeDate = DateFaker.FutureOffset();
        ContactsManagementTimeProvider.SetUtcNow(fakeDate.UtcDateTime);

        var request = AddPersonFaker
            .RuleFor(r => r.LastName, f => lastName)
            .RuleFor(r => r.Company, f => company)
            .Generate();

        // Act
        var personId = await peopleService.AddPerson(request);
        var person = await db.People.FindAsync(personId);

        // Assert
        person.Should().NotBeNull();
        person.FirstName.Should().Be(request.FirstName);
        person.LastName.Should().Be(request.LastName);
        person.Company.Should().Be(request.Company);
        person.UpdatedAt.Should().Be(null);
        person.CreatedAt.Should().Be(ContactsManagementTimeProvider.GetUtcNow().DateTime);
    }

    [Fact]
    public async Task AddPerson_ShouldGenerateUniqueIds()
    {
        // Arrange
        var request1 = AddPersonFaker.Generate();
        var request2 = AddPersonFaker.Generate();

        // Act
        var personId1 = await peopleService.AddPerson(request1);
        var personId2 = await peopleService.AddPerson(request2);

        // Assert
        personId1.Should().NotBe(personId2);
    }

    [Fact]
    public async Task GetPersonByIdAsync_ShouldReturnPerson_WhenIdIsValid()
    {
        // Arrange
        var person = db.People.First();

        // Act
        var result = await peopleService.GetPersonById(person.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(person.Id);
        result.FirstName.Should().Be(person.FirstName);
        result.LastName.Should().Be(person.LastName);
        result.Company.Should().Be(person.Company);
    }

    [Fact]
    public async Task GetPersonByIdAsync_ShouldReturnNull_WhenIdIsInvalid()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var result = await peopleService.GetPersonById(invalidId);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData(1, 5)]
    [InlineData(2, 5)]
    [InlineData(1, 10)]
    public async Task GetPeopleAsync_ShouldReturnCorrectNumberOfRecords_WhenCalledWithValidParameters(
        int pageNumber, int pageSize)
    {
        var result =
            await peopleService.GetPeople(pageNumber, pageSize, CancellationToken.None);

        result.PageNumber.Should().Be(pageNumber);
        result.PageSize.Should().Be(pageSize);
        result.TotalCount.Should().Be(PeopleCount); // Seeded 10 people
        result.Data.Should().HaveCount(pageSize);
    }

    [Fact]
    public async Task GetPeopleAsync_ShouldReturnEmptyData_WhenInvalidPageIsRequested()
    {
        const int pageNumber = 2;
        var pageSize = new Random().Next(PeopleCount + 1, 99);

        var result =
            await peopleService.GetPeople(pageNumber, pageSize, CancellationToken.None);

        result.PageNumber.Should().Be(pageNumber);
        result.PageSize.Should().Be(pageSize);
        result.TotalCount.Should().Be(PeopleCount); // Seeded 10 people
        result.Data.Should().BeEmpty();
    }

    protected override async Task SeedDatabaseAsync()
    {
        // Generate a list of fake Person entities
        var people = PersonFaker.Generate(PeopleCount);

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