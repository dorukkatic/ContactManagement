using ContactReports.Application.Abstractions;
using ContactReports.Application.Helpers;
using ContactReports.Contracts;
using ContactReports.Contracts.Common;
using ContactReports.DataAccess;
using ContactReports.Domain;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ContactReports.Application.Reports;

public class ReportService : IReportService, IInternalReportService
{
    private readonly ContactReportsDbContext db;
    private readonly IEventBus eventBus;
    private readonly ILogger<ReportService> logger;
    private readonly IReportGeneratorFactory reportGeneratorFactory;

    public ReportService(
        ContactReportsDbContext db, 
        IEventBus eventBus, 
        ILogger<ReportService> logger, 
        IReportGeneratorFactory reportGeneratorFactory)
    {
        this.db = db;
        this.eventBus = eventBus;
        this.logger = logger;
        this.reportGeneratorFactory = reportGeneratorFactory;
    }

    public async Task<Result<Guid>> RequestReport(ReportTypeDto requestedReportType)
    {
        var reportType = new ReportTypeMapper().MapReportTypeDtoToReportType(requestedReportType);
        
        var report =
            new Report()
            {
                Type = reportType,
                Statuses = new List<ReportStatus>()
                {
                    new()
                    {
                        Status = Status.Requested,
                        IsEnabled = true
                    }
                }
            };
        
        db.Reports.Add(report);
        await db.SaveChangesAsync();
        
        await eventBus.Publish(new ReportRequestedEvent(report.Id));
        
        return report.Id;
    }

    public async Task<PagedResponse<ReportResponse>> GetReports(PaginationQuery pagination)
    {
        var reportQuery =
            db.Reports.Include(r => r.Statuses).AsNoTracking();

        var totalCount = await reportQuery.LongCountAsync();

        if (!QueryPagingHelper.ShouldFetchData(pagination.PageNumber, pagination.PageSize, totalCount))
        {
            return new PagedResponse<ReportResponse>(
                pagination.PageNumber,
                pagination.PageSize,
                totalCount,
                []);
        }

        var typeMapper = new ReportTypeMapper();
        var statusMapper = new ReportStatusMapper();
        
        var reports =
            await reportQuery
                .OrderByDescending(r => r.CreatedAt)
                .ApplyPaging(pagination.PageNumber, pagination.PageSize)
                .Select(r => 
                    new ReportResponse(
                        r.Id,
                        typeMapper.MapReportTypeToReportTypeDto(r.Type),
                        statusMapper.MapStatusDtoToReportStatus(r.Statuses.First(s => s.IsEnabled).Status),
                        r.Statuses.First(s => s.Status == Status.Requested).CreatedAt,
                        r.Statuses.FirstOrDefault(s => s.Status == Status.Created) == null 
                            ? null
                            : r.Statuses.First(s => s.Status == Status.Created).CreatedAt))
                .ToListAsync();

        return new PagedResponse<ReportResponse>(pagination.PageNumber, pagination.PageSize, totalCount, reports);
    }

    public async Task<Result<ReportType>> GetReportType(Guid reportId)
    {
        var report =
            await db.Reports
                .AsNoTracking()
                .Where(r => r.Id == reportId)
                .FirstOrDefaultAsync();
        
        if (report == null) return Result.Fail("Report not found");

        return report.Type;
    }
    
    public async Task ChangeReportStatus(Guid reportId, Status newStatus)
    {
        var statuses =
            await db.ReportStatuses
                .Where(rs => rs.ReportId == reportId && rs.IsEnabled)
                .ToListAsync();
        
        statuses.ForEach(s => s.IsEnabled = false);
        
        db.ReportStatuses.Add(
            new ReportStatus()
            {
                ReportId = reportId,
                Status = newStatus,
                IsEnabled = true
            });
        
        await db.SaveChangesAsync();
    }
    
    public async Task UpdateReportData(Guid reportId, string data)
    {
        var report =
            await db.Reports.FindAsync(reportId);

        if (report == null)
        {
            logger.LogError("Report with id {reportId} not found", reportId);
            return;
        }
        
        report.Data = data;
        
        await db.SaveChangesAsync();
    }
    
    public async Task GenerateReport(Guid reportId, CancellationToken contextCancellationToken)
    {
        var reportTypeResult = await GetReportType(reportId);
        
        if (reportTypeResult.IsFailed)
        {
            logger.LogError("Report with id {reportId} not found", reportId);
            return;
        }
        
        await ChangeReportStatus(reportId, Status.Creating);
        
        var reportGenerator = reportGeneratorFactory.GetReportGenerator(reportTypeResult.Value);
        var reportData = 
            await reportGenerator.GenerateReport<IEnumerable<PeopleCountByLocation>>(contextCancellationToken);

        await UpdateReportData(reportId, JsonConvert.SerializeObject(reportData.Value));
        
        await ChangeReportStatus(reportId, Status.Created);
    }
}