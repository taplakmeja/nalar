using System.Security.Claims;
using Psikotes.Application.Common.Interfaces;

namespace Psikotes.Web.Services;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public string? UserId => httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;
}
