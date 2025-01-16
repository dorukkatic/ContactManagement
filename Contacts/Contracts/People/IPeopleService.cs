using Contacts.Contracts.Common;

namespace Contacts.Contracts.People;
public interface IPeopleService
{
    Task<Guid> AddPerson(AddPersonRequest request);
    Task<PersonResponse?> GetPersonById(Guid id);
    Task<PagedResponse<PersonResponse>> GetPeople(
        int pageNumber,
        int pageSize,
        PeopleOrderByField orderBy,
        bool isDescending,
        CancellationToken cancellationToken);
}