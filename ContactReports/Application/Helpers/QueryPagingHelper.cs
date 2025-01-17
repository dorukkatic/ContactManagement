namespace ContactReports.Application.Helpers;

public static class QueryPagingHelper
{
    internal static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, int pageNumber, int pageSize)
    {
        return query
            .Skip(CalculateSkip(pageNumber, pageSize))
            .Take(pageSize);
    }

    internal static bool ShouldFetchData(int pageNumber, int pageSize, long totalCount)
    {
        return CalculateSkip(pageNumber, pageSize) < totalCount;
    }
    
    private static int CalculateSkip(int pageNumber, int pageSize)
    {
        return (pageNumber - 1) * pageSize;
    }
}