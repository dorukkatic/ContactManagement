namespace Contacts.Contracts.People;
public interface IPeopleService
{
    Task<Guid> AddPerson(AddPersonRequest request);
}