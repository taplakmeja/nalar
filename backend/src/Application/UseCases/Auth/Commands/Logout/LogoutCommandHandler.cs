using Ardalis.GuardClauses;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Psikotes.Application.Common.Interfaces;

namespace Psikotes.Application.UseCases.Auth.Commands.Logout;

public sealed class LogoutCommandHandler(IAppDbContext dbContext) : ICommandHandler<LogoutCommand, Result>
{
    public async ValueTask<Result> Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrWhiteSpace(command.RefreshToken);

        var stored = await dbContext.RefreshTokens
            .SingleOrDefaultAsync(t => t.Token == command.RefreshToken, cancellationToken);

        if (stored is null || stored.RevokedAtUtc is not null)
        {
            return Result.NoContent();
        }

        stored.Revoke(DateTime.UtcNow);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.NoContent();
    }
}
