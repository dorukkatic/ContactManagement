namespace Contacts.Contracts.People;

public record PersonResponse(
    Guid Id, 
    string FirstName, 
    string LastName, 
    string Company, 
    DateTime CreatedAt, 
    DateTime? UpdatedAt);