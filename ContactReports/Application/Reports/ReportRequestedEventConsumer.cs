using ContactReports.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ContactReports.Application.Reports;

public class ReportRequestedEventConsumer : IConsumer<ReportRequestedEvent>
{
    private readonly ILogger<ReportRequestedEventConsumer> logger;

    public ReportRequestedEventConsumer(ILogger<ReportRequestedEventConsumer> logger)
    {
        this.logger = logger;
    }

    public Task Consume(ConsumeContext<ReportRequestedEvent> context)
    {
        logger.LogInformation("Received report requested event with report id {ReportId}", context.Message.ReportId);
        
        return Task.CompletedTask;    
    }
}