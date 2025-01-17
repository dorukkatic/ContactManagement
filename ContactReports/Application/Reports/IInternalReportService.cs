namespace ContactReports.Application.Reports;

public interface IInternalReportService
{
    Task GenerateReport(Guid reportId, CancellationToken cancellationToken = default);
}    
    
