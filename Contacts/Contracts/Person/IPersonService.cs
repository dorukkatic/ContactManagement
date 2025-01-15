namespace Contacts.Contracts.Person;
public interface IPersonService
{
    Task<Guid> AddPerson(AddPersonRequest request);
}