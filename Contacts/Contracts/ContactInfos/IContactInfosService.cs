namespace Contacts.Contracts.ContactInfos;

public interface IContactInfosService
{
    Task<Guid> AddContactInfo(Guid personId, AddContactInfoRequest request);
}