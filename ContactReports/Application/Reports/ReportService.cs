using ContactReports.Application.Abstractions;
using ContactReports.Contracts;
using ContactReports.DataAccess;
using ContactReports.Domain;
using FluentResults;

namespace ContactReports.Application.Reports;

public class ReportService : IReportService
{
    private readonly ContactReportsDbContext db;
    private readonly IEventBus eventBus;

    public ReportService(ContactReportsDbContext db, IEventBus eventBus)
    {
        this.db = db;
        this.eventBus = eventBus;
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
}