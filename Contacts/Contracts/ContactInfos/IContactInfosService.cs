using FluentResults;

namespace Contacts.Contracts.ContactInfos;

public interface IContactInfosService
{
    Task<Result<Guid>> AddContactInfo(Guid personId, AddContactInfoRequest request);
    Task<Result<Guid>> AddLocation(Guid personId, AddLocationRequest request);
}