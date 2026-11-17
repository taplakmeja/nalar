using Ardalis.Result.AspNetCore;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Psikotes.Application.UseCases.Ads.Dtos;
using Psikotes.Application.UseCases.Ads.Queries.GetAdConfig;

namespace Psikotes.Web.Controllers;

[ApiController]
[Route("api/ads")]
public sealed class AdsController(ISender sender) : ControllerBase
{
    [HttpGet("config")]
    [AllowAnonymous]
    public async Task<ActionResult<AdConfigDto>> GetConfig(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAdConfigQuery(), cancellationToken);
        return result.ToActionResult(this);
    }
}
