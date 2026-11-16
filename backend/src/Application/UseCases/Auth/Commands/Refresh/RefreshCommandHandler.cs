using Ardalis.GuardClauses;
using Ardalis.Result;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Psikotes.Application.Common.Interfaces;
using Psikotes.Domain.Entities;

namespace Psikotes.Application.UseCases.Auth.Commands.Refresh;

public sealed class RefreshCommandHandler(
    UserManager<ApplicationUser> userManager,
    AuthResponseFactory authResponseFactory,
    IAppDbContext dbContext) : ICommandHandler<RefreshCommand, Result<AuthResponse>>
{
    public async ValueTask<Result<AuthResponse>> Handle(RefreshCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrWhiteSpace(command.RefreshToken);

        var stored = await dbContext.RefreshTokens
            .SingleOrDefaultAsync(t => t.Token == command.RefreshToken, cancellationToken);

        if (stored is null || !stored.IsActive)
        {
            return Result.Invalid(new ValidationError
            {
                Identifier = nameof(command.RefreshToken),
                ErrorMessage = "Sesi tidak valid, silakan masuk kembali."
            });
        }

        var user = await userManager.FindByIdAsync(stored.UserId);
        if (user is null)
        {
            return Result.Invalid(new ValidationError
            {
                Identifier = nameof(command.RefreshToken),
                ErrorMessage = "Sesi tidak valid, silakan masuk kembali."
            });
        }

        stored.Revoke(DateTime.UtcNow);

        var response = await authResponseFactory.IssueAsync(user, cancellationToken);

        stored.SetReplacedByToken(response.RefreshToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(response);
    }
}
