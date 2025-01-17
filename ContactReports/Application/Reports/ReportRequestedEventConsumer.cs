using ContactReports.Contracts;
using ContactReports.Domain;
using MassTransit;
using Newtonsoft.Json;

namespace ContactReports.Application.Reports;

public class ReportRequestedEventConsumer : IConsumer<ReportRequestedEvent>
{
    private readonly IInternalReportService reportService;

    public ReportRequestedEventConsumer(IInternalReportService reportService)
    {
        this.reportService = reportService;
    }

    public async Task Consume(ConsumeContext<ReportRequestedEvent> context)
    {
        var reportId=  context.Message.ReportId;

        await reportService.GenerateReport(reportId, context.CancellationToken);
    }
}