using ContactReports.Domain;

namespace ContactReports.Application.Reports;

public interface IReportGeneratorFactory
{
    public IReportGenerator GetReportGenerator(ReportType reportType);
}