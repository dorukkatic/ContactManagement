using Contacts.Contracts.ContactInfos;
using Riok.Mapperly.Abstractions;

namespace Contacts.Application.ContactInfos;

[Mapper]
public partial class ContactInfoMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public partial ContactInfoResponse ContactInfoToContactInfoResponse(Domain.ContactInfo contactInfo);

    [MapperRequiredMapping(RequiredMappingStrategy.Source)]
    public partial Domain.ContactInfo ContactInfoRequestToContactInfo(AddContactInfoRequest contactInfoRequest, Guid personId);
}