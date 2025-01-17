namespace ContactReports.Domain;

public class Report : BaseEntity
{
    public required ReportType Type { get; set; }
    public string? Data { get; set; }
    public required IEnumerable<ReportStatus> Statuses { get; set; }
}