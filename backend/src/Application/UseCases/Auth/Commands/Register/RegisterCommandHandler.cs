using Ardalis.GuardClauses;
using Ardalis.Result;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Psikotes.Application.Common.Interfaces;
using Psikotes.Domain.Common;
using Psikotes.Domain.Entities;

namespace Psikotes.Application.UseCases.Auth.Commands.Register;

public sealed class RegisterCommandHandler(
    UserManager<ApplicationUser> userManager,
    AuthResponseFactory authResponseFactory,
    IAppDbContext dbContext) : ICommandHandler<RegisterCommand, Result<AuthResponse>>
{
    public async ValueTask<Result<AuthResponse>> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrWhiteSpace(command.Email);
        Guard.Against.NullOrWhiteSpace(command.Password);
        Guard.Against.NullOrWhiteSpace(command.DisplayName);

        var existing = await userManager.FindByEmailAsync(command.Email);
        if (existing is not null)
        {
            return Result.Invalid(new ValidationError
            {
                Identifier = nameof(command.Email),
                ErrorMessage = "Email sudah terdaftar."
            });
        }

        var user = new ApplicationUser
        {
            UserName = command.Email,
            Email = command.Email,
            DisplayName = command.DisplayName,
            CreatedAtUtc = DateTime.UtcNow
        };

        var createResult = await userManager.CreateAsync(user, command.Password);
        if (!createResult.Succeeded)
        {
            return Result.Invalid(createResult.Errors.Select(e => new ValidationError
            {
                Identifier = e.Code,
                ErrorMessage = e.Description
            }));
        }

        await userManager.AddToRoleAsync(user, Roles.User);

        var response = await authResponseFactory.IssueAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(response);
    }
}
