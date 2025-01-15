namespace Contacts.Domain;
public class Person : BaseEntity
{
    public required string FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Company { get; set; }
}

