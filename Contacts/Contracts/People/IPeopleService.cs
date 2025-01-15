namespace Contacts.Contracts.People;
public interface IPeopleService
{
    Task<Guid> AddPerson(AddPersonRequest request);
    Task<PersonResponse?> GetPersonById(Guid id);
}