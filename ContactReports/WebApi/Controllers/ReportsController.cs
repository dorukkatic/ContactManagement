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

    [HttpPost]
    [Route("")]
    public async Task<IActionResult> RequestReport(
        [FromBody] ReportTypeDto requestedReportType)
    {
        var result = await reportService.RequestReport(requestedReportType);
        return
            result.IsFailed
                ? result.ToActionResult()
                : CreatedAtAction(nameof(GetReport), new { id = result.Value }, null);
    }
    
    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetReport(
        [FromRoute] Guid id)
    {
        var result = await reportService.GetReport(id);

        if (result.IsFailed) return NotFound();
        
        return Ok(result.Value);
    }
    
    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetReports([FromQuery] PaginationQuery pagination)
    {
        var reports = await reportService.GetReports(pagination);
        return Ok(reports);
    }
}