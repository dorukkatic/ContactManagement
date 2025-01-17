using ContactReports.Domain;
using FluentResults;

namespace ContactReports.Application.Reports;

public interface IReportGenerator
{
    public bool CanGenerate(ReportType reportType);
    public Task<Result<T>> GenerateReport<T>(CancellationToken cancellationToken = default);
}