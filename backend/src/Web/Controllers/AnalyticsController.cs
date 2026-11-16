using Ardalis.Result.AspNetCore;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Psikotes.Application.UseCases.Analytics.Dtos;
using Psikotes.Application.UseCases.Analytics.Queries.GetDashboard;

namespace Psikotes.Web.Controllers;

[ApiController]
[Route("api/analytics")]
[Authorize]
public sealed class AnalyticsController(ISender sender) : ControllerBase
{
    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardDto>> GetDashboard(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetDashboardQuery(), cancellationToken);
        return result.ToActionResult(this);
    }
}
