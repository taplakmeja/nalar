using Ardalis.Result.AspNetCore;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Psikotes.Application.UseCases.Admin.Users;
using Psikotes.Domain.Common;

namespace Psikotes.Web.Controllers.Admin;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = Roles.Administrator)]
public sealed class UsersController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AdminUserDto>>> List(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ListUsersQuery(), cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpPut("{id}/roles")]
    public async Task<ActionResult> SetRoles(string id, [FromBody] SetRolesRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new SetUserRolesCommand(id, request.Roles), cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpPut("{id}/lockout")]
    public async Task<ActionResult> SetLockout(string id, [FromBody] SetLockoutRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new SetUserLockoutCommand(id, request.Locked), cancellationToken);
        return result.ToActionResult(this);
    }

    public sealed record SetRolesRequest(IReadOnlyList<string> Roles);

    public sealed record SetLockoutRequest(bool Locked);
}
