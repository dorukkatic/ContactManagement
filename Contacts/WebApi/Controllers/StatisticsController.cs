using Contacts.Contracts.Statistics;
using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace Contacts.WebApi.Controllers;

[ApiController]
[ApiExplorerSettings(GroupName = "Statistics")]
[Route("statistics")]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticsService statisticsService;

    public StatisticsController(IStatisticsService statisticsService)
    {
        this.statisticsService = statisticsService;
    }

    [HttpGet]
    [Route("people-count-by-locations")]
    public async Task<IActionResult> GetPeopleCountByLocations(
        CancellationToken cancellationToken = default)
    {
        var peopleCountByLocation = 
            await statisticsService.GetPeopleCountByLocations(cancellationToken);
        
        return peopleCountByLocation.ToActionResult();
    }
}