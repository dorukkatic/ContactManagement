namespace Contacts.Contracts.People;

public record PeopleOrderByQuery(PeopleOrderByField OrderBy, bool IsDescending);
