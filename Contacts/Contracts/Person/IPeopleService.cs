namespace Contacts.Contracts.Person;
public interface IPeopleService
{
    Task<Guid> AddPerson(AddPersonRequest request);
}