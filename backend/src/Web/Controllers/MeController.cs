using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Psikotes.Application.UseCases.Auth;

namespace Psikotes.Web.Controllers;

[ApiController]
[Route("api/me")]
[Authorize]
public sealed class MeController : ControllerBase
{
    [HttpGet]
    public ActionResult<UserDto> Me()
    {
        var user = new UserDto(
            User.FindFirst("sub")?.Value ?? User.Identity?.Name ?? string.Empty,
            User.FindFirst("email")?.Value ?? string.Empty,
            User.FindFirst("displayName")?.Value ?? string.Empty,
            User.Claims
                .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList());

        return Ok(user);
    }
}
