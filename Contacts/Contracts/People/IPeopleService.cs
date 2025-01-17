using Contacts.Contracts.Common;
using FluentResults;

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

    Task<Result> DeletePerson(Guid id);
}