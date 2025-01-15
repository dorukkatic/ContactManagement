namespace Contacts.Domain;
public class Person
{
    public Guid Id { get; set; }
    public required string FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Company { get; set; }
}

