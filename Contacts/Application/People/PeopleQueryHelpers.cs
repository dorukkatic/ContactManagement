using Contacts.Contracts.People;
using Contacts.Domain;

namespace Contacts.Application.People;
internal static class PeopleQueryHelpers
{
    internal static IQueryable<Person> ApplyOrdering(
        this IQueryable<Person> people,
        PeopleOrderByField orderBy,
        bool isDescending)
    {
        return (orderBy, isDescending) switch
        {
            (PeopleOrderByField.FirstName, true) => people.OrderByDescending(p => p.FirstName),
            (PeopleOrderByField.FirstName, false) => people.OrderBy(p => p.FirstName),
            (PeopleOrderByField.LastName, true) => people.OrderByDescending(p => p.LastName),
            (PeopleOrderByField.LastName, false) => people.OrderBy(p => p.LastName),
            (PeopleOrderByField.Company, true) => people.OrderByDescending(p => p.Company),
            (PeopleOrderByField.Company, false) => people.OrderBy(p => p.Company),
            (PeopleOrderByField.CreatedAt, true) => people.OrderByDescending(p => p.CreatedAt),
            (PeopleOrderByField.CreatedAt, false) => people.OrderBy(p => p.CreatedAt),
            (PeopleOrderByField.UpdatedAt, true) => people.OrderByDescending(p => p.UpdatedAt),
            (PeopleOrderByField.UpdatedAt, false) => people.OrderBy(p => p.UpdatedAt),
            _ => people.OrderBy(p => p.CreatedAt)
        };
    }

    internal static IQueryable<Person> ApplyPaging(
        this IQueryable<Person> people,
        int pageNumber,
        int pageSize)
    {
        return people.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }

}
