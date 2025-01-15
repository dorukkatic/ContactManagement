namespace Contacts.Domain;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public void SetCreationTimestamp(TimeProvider timeProvider)
    {
        CreatedAt = timeProvider.GetUtcNow().UtcDateTime;
    }

    public void SetModificationTimestamp(TimeProvider timeProvider)
    {
        UpdatedAt = timeProvider.GetUtcNow().UtcDateTime;
    }
}