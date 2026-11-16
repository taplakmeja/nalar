using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Psikotes.Application.Auth.Dtos;
using Psikotes.Application.Common.Interfaces;
using Psikotes.Domain.Entities;

namespace Psikotes.Application.Auth;

public sealed class AuthService(
    UserManager<ApplicationUser> userManager,
    IAppDbContext dbContext,
    ITokenService tokenService)
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await userManager.FindByEmailAsync(request.Email);
        if (existing is not null)
        {
            throw new AuthException("Email sudah terdaftar.");
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            DisplayName = request.DisplayName,
            CreatedAtUtc = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            throw new AuthException(string.Join(" ", result.Errors.Select(e => e.Description)));
        }

        await userManager.AddToRoleAsync(user, Domain.Common.Roles.User);

        return await IssueTokensAsync(user, cancellationToken);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            throw new AuthException("Email atau kata sandi salah.");
        }

        return await IssueTokensAsync(user, cancellationToken);
    }

    public async Task<AuthResponse> RefreshAsync(RefreshRequest request, CancellationToken cancellationToken = default)
    {
        var stored = await dbContext.RefreshTokens
            .SingleOrDefaultAsync(t => t.Token == request.RefreshToken, cancellationToken);

        if (stored is null || !stored.IsActive)
        {
            throw new AuthException("Sesi tidak valid, silakan masuk kembali.");
        }

        var user = await userManager.FindByIdAsync(stored.UserId)
            ?? throw new AuthException("Sesi tidak valid, silakan masuk kembali.");

        stored.RevokedAtUtc = DateTime.UtcNow;

        return await IssueTokensAsync(user, cancellationToken);
    }

    public async Task LogoutAsync(RefreshRequest request, CancellationToken cancellationToken = default)
    {
        var stored = await dbContext.RefreshTokens
            .SingleOrDefaultAsync(t => t.Token == request.RefreshToken, cancellationToken);

        if (stored is not null && stored.RevokedAtUtc is null)
        {
            stored.RevokedAtUtc = DateTime.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task<AuthResponse> IssueTokensAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
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
        await dbContext.SaveChangesAsync(cancellationToken);

        return new AuthResponse(
            accessToken,
            refreshToken,
            new UserDto(user.Id, user.Email!, user.DisplayName, roles.ToList()));
    }
}

public sealed class AuthException(string message) : Exception(message);
