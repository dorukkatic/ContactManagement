using Contacts.Contracts.Common;
using Contacts.Contracts.ContactInfos;
using Contacts.Contracts.People;
using Riok.Mapperly.Abstractions;

namespace Contacts.Application.People;

[Mapper]
public partial class PersonMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Source)]
    public partial Domain.Person AddPersonRequestToPerson(AddPersonRequest request);
    
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public partial PersonResponse PersonToPersonResponse(Domain.Person person);
    
    [MapperRequiredMapping(RequiredMappingStrategy.None)]
    public partial PersonDetailResponse PersonToPersonDetailResponse(
        Domain.Person person, 
        PagedResponse<ContactInfoResponse> contactInfos);
}