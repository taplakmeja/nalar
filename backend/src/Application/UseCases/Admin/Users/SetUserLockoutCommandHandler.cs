using Ardalis.GuardClauses;
using Ardalis.Result;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Psikotes.Domain.Entities;

namespace Psikotes.Application.UseCases.Admin.Users;

public sealed class SetUserLockoutCommandHandler(UserManager<ApplicationUser> userManager)
    : ICommandHandler<SetUserLockoutCommand, Result>
{
    public async ValueTask<Result> Handle(SetUserLockoutCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrWhiteSpace(command.UserId);

        var user = await userManager.FindByIdAsync(command.UserId);
        if (user is null)
        {
            return Result.NotFound("Pengguna tidak ditemukan.");
        }

        var result = await userManager.SetLockoutEndDateAsync(
            user,
            command.Locked ? DateTimeOffset.MaxValue : null);
        if (!result.Succeeded)
        {
            return Result.Error(new ErrorList(
                result.Errors.Select(e => e.Description).ToList(),
                null));
        }

        return Result.Success();
    }
}
