namespace Contacts.Domain;

public class ContactInfo : BaseEntity
{
    public required Guid PersonId { get; set; }
    public required ContactInfoType Type { get; set; }
    public required string Value { get; set; }
    public Person? Person { get; set; }
}