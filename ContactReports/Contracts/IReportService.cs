using ContactReports.Contracts.Common;
using FluentResults;

namespace ContactReports.Contracts;

public interface IReportService
{
    Task<Result<Guid>> RequestReport(ReportTypeDto requestedReportType);
    Task<PagedResponse<ReportResponse>> GetReports(PaginationQuery pagination);
    Task<Result<ReportDetailResponse>> GetReport(Guid id);
}