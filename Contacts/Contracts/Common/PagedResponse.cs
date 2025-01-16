namespace Contacts.Contracts.Common;

public record PagedResponse<T>(int PageNumber, int PageSize, long TotalCount, IEnumerable<T> Data);