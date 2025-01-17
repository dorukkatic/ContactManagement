using Contacts.Application.Helpers;
using Contacts.Contracts.Common;
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

    public async Task<Result> DeleteContactInfo(Guid id)
    {
        var contactInfo = await db.ContactInfos.FindAsync(id);
        if(contactInfo == null) return Result.Fail("Contact info doesn't exist");
        
        db.ContactInfos.Remove(contactInfo);
        await db.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<PagedResponse<ContactInfoResponse>> GetContactInfos(Guid personId, int pageNumber, int pageSize)
    {
        var mapper = new ContactInfoMapper();

        var contactInfosQuery =
            db.ContactInfos
                .AsNoTracking()
                .Where(ci => ci.PersonId == personId);
        
        var total = await contactInfosQuery.LongCountAsync();
        
        if (!QueryPagingHelper.ShouldFetchData(pageNumber, pageSize, total))
        {
            return new PagedResponse<ContactInfoResponse>(
                pageNumber,
                pageSize,
                total,
                new List<ContactInfoResponse>());
        }
        
        var contactInfos =
            await contactInfosQuery
                .OrderBy(ci => ci.Type)
                .ThenBy(ci => ci.Value)
                .ApplyPaging(pageNumber, pageSize)
                .Select(ci => mapper.ContactInfoToContactInfoResponse(ci))
                .ToListAsync();
        
        return new PagedResponse<ContactInfoResponse>(
            pageNumber,
            pageSize,
            total,
            contactInfos);
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