using ContactReports.Domain;
using FluentResults;

namespace ContactReports.Application.Reports;

public interface IReportGenerator
{
    public bool CanGenerate(ReportType reportType);
    public Task<Result<object>> GenerateReport(CancellationToken cancellationToken = default);
}