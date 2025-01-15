using Contacts.Contracts.Person;
using Riok.Mapperly.Abstractions;

namespace Contacts.Application.Person;

[Mapper]
public partial class PersonMapper
{
    public partial Domain.Person AddPersonRequestToPerson(AddPersonRequest request);
}