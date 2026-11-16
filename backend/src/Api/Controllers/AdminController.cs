using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Psikotes.Domain.Common;

namespace Psikotes.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = Roles.Administrator)]
public sealed class AdminController : ControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Ok(new { message = "admin-ok" });
}
