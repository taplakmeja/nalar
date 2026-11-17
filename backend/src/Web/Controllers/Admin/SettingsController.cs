using Ardalis.Result.AspNetCore;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Psikotes.Application.UseCases.Admin.Settings;
using Psikotes.Domain.Common;

namespace Psikotes.Web.Controllers.Admin;

[ApiController]
[Route("api/admin/settings")]
[Authorize(Roles = Roles.Administrator)]
public sealed class SettingsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AppSettingDto>>> List(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetSettingsQuery(), cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpPut("{key}")]
    public async Task<ActionResult<AppSettingDto>> Update(string key, [FromBody] UpdateSettingRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new UpdateSettingCommand(key, request.Value), cancellationToken);
        return result.ToActionResult(this);
    }

    public sealed record UpdateSettingRequest(string Value);
}
