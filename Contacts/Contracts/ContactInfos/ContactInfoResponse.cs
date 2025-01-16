namespace Contacts.Contracts.ContactInfos;

public record ContactInfoResponse(
    Guid Id, 
    ContactInfoTypeDTO Type, 
    string Value, 
    Guid PersonId,
    DateTime CreatedAt, 
    DateTime? UpdatedAt);