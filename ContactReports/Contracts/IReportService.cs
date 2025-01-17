using FluentResults;

namespace ContactReports.Contracts;

public interface IReportService
{
    Task<Result<Guid>> RequestReport(ReportTypeDto requestedReportType);
}