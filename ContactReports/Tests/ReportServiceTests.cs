using Bogus;
using Castle.Core.Logging;
using ContactReports.Application.Abstractions;
using ContactReports.Application.Reports;
using ContactReports.Contracts;
using ContactReports.Contracts.Common;
using ContactReports.DataAccess;
using ContactReports.Domain;
using FluentAssertions;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace ContactReports.Application.Tests.Unit;

public class ReportServiceTests : ReportsTestBase, IAsyncLifetime
{
    private readonly IEventBus eventBusMock;
    private readonly ReportService reportService;
    private readonly IReportGeneratorFactory reportGeneratorFactoryMock;
    private readonly ILogger<ReportService> logger;

    public ReportServiceTests()
    {
        eventBusMock = Substitute.For<IEventBus>();
        reportGeneratorFactoryMock = Substitute.For<IReportGeneratorFactory>();
        logger = Substitute.For<ILogger<ReportService>>();
        reportService = new ReportService(db, eventBusMock, logger, reportGeneratorFactoryMock);
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
    
    [Fact]
    public async Task GetReportType_ShouldReturnReportType()
    {
        var report = new Report
        {
            Type = ReportType.PeopleByLocation,
            Statuses = new List<ReportStatus> { new() { Status = Status.Requested, IsEnabled = true } }
        };
        db.Reports.Add(report);
        await db.SaveChangesAsync();

        var result = await reportService.GetReportType(report.Id);

        result.Value.Should().Be(ReportType.PeopleByLocation);
    }
    
    [Fact]
    public async Task GetReportType_ShouldReturnFailWhenReportNotFound()
    {
        var result = await reportService.GetReportType(Guid.NewGuid());

        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Be("Report not found");
    }
    
    [Fact]
    public async Task ChangeReportStatus_ShouldUpdateStatus()
    {
        var report = new Report
        {
            Type = ReportType.PeopleByLocation,
            Statuses = new List<ReportStatus> { new() { Status = Status.Requested, IsEnabled = true } }
        };
        db.Reports.Add(report);
        await db.SaveChangesAsync();

        await reportService.ChangeReportStatus(report.Id, Status.Creating);

        var updatedStatuses = await db.ReportStatuses.Where(rs => rs.ReportId == report.Id).ToListAsync();
        updatedStatuses.Should().ContainSingle(s => s.Status == Status.Creating && s.IsEnabled);
    }
    
    [Fact]
    public async Task UpdateReportData_ShouldUpdateData()
    {
        var report = new Report
        {
            Type = ReportType.PeopleByLocation,
            Statuses = new List<ReportStatus> { new ReportStatus { Status = Status.Requested, IsEnabled = true } }
        };
        db.Reports.Add(report);
        await db.SaveChangesAsync();

        const string newData = "Updated Data";
        await reportService.UpdateReportData(report.Id, newData);

        var updatedReport = await db.Reports.FirstOrDefaultAsync(r => r.Id == report.Id);
        updatedReport?.Data.Should().Be(newData);
    }
    
    [Fact]
    public async Task GenerateReport_ShouldGenerateAndSaveReportData()
    {
        var report = new Report
        {
            Type = ReportType.PeopleByLocation,
            Statuses = new List<ReportStatus> { new() { Status = Status.Requested, IsEnabled = true } }
        };
        db.Reports.Add(report);
        await db.SaveChangesAsync();

        var mockDataResult = Result.Ok<IEnumerable<PeopleCountByLocation>>(new List<PeopleCountByLocation>());

        var reportGeneratorMock = Substitute.For<IReportGenerator>();
        reportGeneratorMock.GenerateReport<IEnumerable<PeopleCountByLocation>>(Arg.Any<CancellationToken>())
            .Returns(mockDataResult);
        
        reportGeneratorFactoryMock.GetReportGenerator(ReportType.PeopleByLocation).Returns(reportGeneratorMock);

        await reportService.GenerateReport(report.Id, CancellationToken.None);

        var updatedReport = await db.Reports.FirstOrDefaultAsync(r => r.Id == report.Id);
        updatedReport?.Data.Should().NotBeNullOrEmpty();
        var updatedStatuses = await db.ReportStatuses.Where(rs => rs.ReportId == report.Id).ToListAsync();
        updatedStatuses.Should().ContainSingle(s => s.Status == Status.Created && s.IsEnabled);
    }
    
    [Fact]
    public async Task GetReports_ShouldReturnPagedReports()
    {
        var report = new Report
        {
            Type = ReportType.PeopleByLocation,
            Statuses = new List<ReportStatus> { new() { Status = Status.Requested, IsEnabled = true } }
        };
        db.Reports.Add(report);
        await db.SaveChangesAsync();

        var pagination = new PaginationQuery(1, 10);
        var result = await reportService.GetReports(pagination);

        result.Data.Should().HaveCount(1);
        result.Data.First().Id.Should().Be(report.Id);
    }
    
    [Fact]
    public async Task GetReports_ShouldReturnEmptyWhenNoReports()
    {
        var pagination = new PaginationQuery(1, 10);
        var result = await reportService.GetReports(pagination);

        result.Data.Should().BeEmpty();
    }
    
    [Fact]
    public async Task GetReportType_ShouldReturnFailWhenReportIdIsEmpty()
    {
        var result = await reportService.GetReportType(Guid.Empty);

        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Be("Report not found");
    }
    
    [Fact]
    public async Task UpdateReportData_ShouldFailWhenReportNotFound()
    {
        var reportId = Guid.NewGuid();
        const string newData = "Updated Data";

        await reportService.UpdateReportData(reportId, newData);

        var updatedReport = await db.Reports.FirstOrDefaultAsync(r => r.Id == reportId);
        updatedReport.Should().BeNull();
    }
    
    [Fact]
    public async Task GenerateReport_ShouldFailWhenReportNotFound()
    {
        var reportId = Guid.NewGuid();

        await reportService.GenerateReport(reportId, CancellationToken.None);

        var updatedReport = await db.Reports.FirstOrDefaultAsync(r => r.Id == reportId);
        updatedReport.Should().BeNull();
    }

}