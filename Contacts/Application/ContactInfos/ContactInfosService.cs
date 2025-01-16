using Contacts.Contracts.ContactInfos;
using Contacts.DataAccess;

namespace Contacts.Application.ContactInfos;

public class ContactInfosService : IContactInfosService
{
    private readonly ContactsDbContext db;

    public ContactInfosService(ContactsDbContext db)
    {
        this.db = db;
    }
    
    public async Task<Guid> AddContactInfo(Guid personId, AddContactInfoRequest request)
    {
        var mapper = new ContactInfoMapper();

        var contactInfo = mapper.ContactInfoRequestToContactInfo(request, personId);
        contactInfo.PersonId = personId;

        db.ContactInfos.Add(contactInfo);

        await db.SaveChangesAsync();

        return contactInfo.Id;
    }
}