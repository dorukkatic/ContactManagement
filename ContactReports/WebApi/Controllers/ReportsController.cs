using ContactReports.Contracts;
using ContactReports.Contracts.Common;
using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace ContactReports.WebApi.Controllers;

[ApiController]
[ApiExplorerSettings(GroupName = "Reports")]
[Route("reports")]
public class ReportsController : ControllerBase
{
    private readonly IReportService reportService;

    public ReportsController(IReportService reportService)
    {
        this.reportService = reportService;
    }

    /// <summary>
    /// Requests a new report generation
    /// </summary>
    /// <param name="requestedReportType">
    /// Type of report to generate.
    /// Available values:
    /// - 0: PeopleByLocation
    /// </param>
    /// <response code="201">Returns the URL to check report status</response>
    /// <response code="400">If the request is invalid</response>
    [HttpPost]
    [Route("")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RequestReport(
        [FromBody] ReportTypeDto requestedReportType)
    {
        var result = await reportService.RequestReport(requestedReportType);
        return
            result.IsFailed
                ? result.ToActionResult()
                : CreatedAtAction(nameof(GetReport), new { id = result.Value }, null);
    }

    /// <summary>
    /// Gets a specific report by its ID
    /// </summary>
    /// <param name="id">The report ID</param>
    /// <returns>
    /// Report details including:
    /// - ID (guid)
    /// - Type (enum: 0 = PeopleByLocation)
    /// - Status (enum: 0 = Requested, 1 = Creating, 2 = Created, 3 = Failed)
    /// - RequestedAt (datetime)
    /// - CreatedAt (datetime)
    /// - Data (object, report-specific content)
    /// </returns>
    /// <response code="200">Returns the report details</response>
    /// <response code="404">If report is not found</response>
    [HttpGet]
    [Route("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReport(
        [FromRoute] Guid id)
    {
        var result = await reportService.GetReport(id);

        if (result.IsFailed) return NotFound();

        return Ok(result.Value);
    }

    /// <summary>
    /// Gets a paginated list of reports
    /// </summary>
    /// <param name="pagination">Page number and size for pagination</param>
    /// <returns>
    /// Paginated list containing:
    /// - PageNumber (int)
    /// - PageSize (int)
    /// - TotalCount (long)
    /// - Items (array of report summaries)
    /// </returns>
    /// <response code="200">Returns the paginated list of reports</response>
    [HttpGet]
    [Route("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReports([FromQuery] PaginationQuery pagination)
    {
        var reports = await reportService.GetReports(pagination);
        return Ok(reports);
    }
}