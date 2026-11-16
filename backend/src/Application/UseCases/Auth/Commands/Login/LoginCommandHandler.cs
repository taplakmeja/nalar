using Ardalis.GuardClauses;
using Ardalis.Result;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Psikotes.Application.Common.Interfaces;
using Psikotes.Domain.Entities;

namespace Psikotes.Application.UseCases.Auth.Commands.Login;

public sealed class LoginCommandHandler(
    UserManager<ApplicationUser> userManager,
    AuthResponseFactory authResponseFactory,
    IAppDbContext dbContext) : ICommandHandler<LoginCommand, Result<AuthResponse>>
{
    public async ValueTask<Result<AuthResponse>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrWhiteSpace(command.Email);
        Guard.Against.NullOrWhiteSpace(command.Password);

        var user = await userManager.FindByEmailAsync(command.Email);
        if (user is null || !await userManager.CheckPasswordAsync(user, command.Password))
        {
            return Result.Unauthorized("Email atau kata sandi salah.");
        }

        var response = await authResponseFactory.IssueAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(response);
    }
}
