namespace Contacts.Contracts.ContactInfos;

public record AddContactInfoRequest(
    ContactInfoTypeDTO Type,
    string Value);