using Bogus;
using ContactReports.Application.Abstractions;
using ContactReports.Application.Reports;
using ContactReports.Contracts;
using ContactReports.DataAccess;
using ContactReports.Domain;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ContactReports.Application.Tests.Unit;

public class ReportServiceTests : ReportsTestBase, IAsyncLifetime
{
    private readonly IEventBus eventBusMock;
    private readonly ReportService reportService;

    public ReportServiceTests()
    {
        eventBusMock = Substitute.For<IEventBus>();
        reportService = new ReportService(db, eventBusMock);
    }

    public async Task InitializeAsync()
    {
        // Initialization logic if needed
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(ReportTypeDto.PeopleByLocation)]
    public async Task RequestReport_ShouldAddReportToDatabase(ReportTypeDto reportType)
    {
        var result = (await reportService.RequestReport(reportType)).Value;
        
        var report = 
            await db.Reports.Include(r => r.Statuses)
                .FirstOrDefaultAsync(r => r.Id == result);

        report?.Type.Should().HaveSameValueAs(reportType);
        report?.Statuses.Should().ContainSingle(s => s.Status == Status.Requested && s.IsEnabled);
    }
    
    [Theory]
    [InlineData(ReportTypeDto.PeopleByLocation)]
    public async Task RequestReport_ShouldReturnValidGuid(ReportTypeDto reportType)
    {
        var result = (await reportService.RequestReport(reportType)).Value;

        result.Should().NotBeEmpty();
    }
    
    [Theory]
    [InlineData(ReportTypeDto.PeopleByLocation)]
    public async Task RequestReport_ShouldPublishReportRequestedEvent(ReportTypeDto reportType)
    {
        var result = (await reportService.RequestReport(reportType)).Value;

        await eventBusMock.Received(1).Publish(Arg.Is<ReportRequestedEvent>(e => e.ReportId == result));
    }

}