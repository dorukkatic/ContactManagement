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
}