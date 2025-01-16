using Contacts.Contracts.ContactInfos;
using Contacts.DataAccess;
using Contacts.Domain;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Contacts.Application.ContactInfos;

public class ContactInfosService : IContactInfosService
{
    private readonly ContactsDbContext db;

    public ContactInfosService(ContactsDbContext db)
    {
        this.db = db;
    }
    
    public async Task<Result<Guid>> AddContactInfo(Guid personId, AddContactInfoRequest request)
    {
        var mapper = new ContactInfoMapper();
        
        var contactInfo = mapper.ContactInfoRequestToContactInfo(request, personId);

        return await AddContactInfo(contactInfo);
    }

    public async Task<Result<Guid>> AddLocation(Guid personId, AddLocationRequest request)
    {
        var mapper = new ContactInfoMapper();

        if (await PersonHasLocation(personId))
        {
            return Result.Fail("Person already has a location");
        }
        
        var location = mapper.LocationRequestToContactInfo(request, personId);
        location.Value = location.Value.ToUpper();

        return await AddContactInfo(location);
    }

    private async Task<Result<Guid>> AddContactInfo(ContactInfo contactInfo)
    {        
        var isPersonExist = await db.People.AnyAsync(p => p.Id == contactInfo.PersonId);
        
        if(!isPersonExist) return Result.Fail("Person doesn't exist");
        
        db.ContactInfos.Add(contactInfo);

        await db.SaveChangesAsync();

        return contactInfo.Id;
    }

    private async Task<bool> PersonHasLocation(Guid perId)
    {
        return await db.ContactInfos
            .AnyAsync(ci => ci.PersonId == perId && ci.Type == ContactInfoType.Location);
    }
}