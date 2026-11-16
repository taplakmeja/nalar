using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Identity;
using Psikotes.Application.Common.Interfaces;
using Psikotes.Domain.Entities;

namespace Psikotes.Application.UseCases.Auth;

public sealed class AuthResponseFactory(
    UserManager<ApplicationUser> userManager,
    ITokenService tokenService,
    IAppDbContext dbContext)
{
    public async ValueTask<AuthResponse> IssueAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        Guard.Against.Null(user);

        var roles = await userManager.GetRolesAsync(user);
        var accessToken = tokenService.GenerateAccessToken(user, roles);
        var refreshToken = tokenService.GenerateRefreshToken();

        dbContext.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshToken,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(14)
        });

        return new AuthResponse(
            accessToken,
            refreshToken,
            new UserDto(user.Id, user.Email!, user.DisplayName, roles.ToList()));
    }
}
