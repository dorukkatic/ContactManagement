using Contacts.Contracts.People;
using Riok.Mapperly.Abstractions;

namespace Contacts.Application.People;

[Mapper]
public partial class PersonMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Source)]
    public partial Domain.Person AddPersonRequestToPerson(AddPersonRequest request);
}