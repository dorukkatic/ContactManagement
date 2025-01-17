namespace ContactReports.Contracts.Common;

public record PaginationQuery(int PageNumber = 1, int PageSize = 10);