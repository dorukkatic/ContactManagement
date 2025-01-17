using ContactReports.Domain;

namespace ContactReports.Domain;

public class ReportStatus : BaseEntity
{
    public Status Status { get; set; }
    public bool IsEnabled { get; set; }
    public Guid ReportId { get; set; }
    public Report? Report { get; set; }
}