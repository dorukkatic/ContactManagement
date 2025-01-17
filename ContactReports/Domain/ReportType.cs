namespace ContactReports.Domain;

public enum ReportType
{
    PeopleByLocation
}

public static class ReportTypeExtensions
{
    public static Type GetReportObjectType(this ReportType reportType)
    {
        return reportType switch
        {
            ReportType.PeopleByLocation => typeof(IEnumerable<PeopleCountByLocation>),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}