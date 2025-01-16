using Contacts.Contracts.ContactInfos;
using Contacts.Domain;
using Riok.Mapperly.Abstractions;

namespace Contacts.Application.ContactInfos;

[Mapper]
public partial class ContactInfoMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public partial ContactInfoResponse ContactInfoToContactInfoResponse(Domain.ContactInfo contactInfo);

    [MapperRequiredMapping(RequiredMappingStrategy.Source)]
    public partial Domain.ContactInfo ContactInfoRequestToContactInfo(
        AddContactInfoRequest contactInfoRequest, 
        Guid personId);
    
    [MapperRequiredMapping(RequiredMappingStrategy.Source)]
    [MapValue(nameof(Domain.ContactInfo.Type), Domain.ContactInfoType.Location)]
    public partial Domain.ContactInfo LocationRequestToContactInfo(
        AddLocationRequest locationRequest, 
        Guid personId);
}