using Ardalis.GuardClauses;
using Ardalis.Result;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Psikotes.Domain.Common;
using Psikotes.Domain.Entities;

namespace Psikotes.Application.UseCases.Admin.Users;

public sealed class SetUserRolesCommandHandler(UserManager<ApplicationUser> userManager)
    : ICommandHandler<SetUserRolesCommand, Result>
{
    public async ValueTask<Result> Handle(SetUserRolesCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrWhiteSpace(command.UserId);

        var user = await userManager.FindByIdAsync(command.UserId);
        if (user is null)
        {
            return Result.NotFound("Pengguna tidak ditemukan.");
        }

        var currentRoles = await userManager.GetRolesAsync(user);
        var desiredRoles = command.Roles
            .Select(r => Roles.All.First(known => known.Equals(r, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles.Except(desiredRoles));
        if (!removeResult.Succeeded)
        {
            return Result.Error(new ErrorList(
                removeResult.Errors.Select(e => e.Description).ToList(),
                null));
        }

        var addResult = await userManager.AddToRolesAsync(user, desiredRoles.Except(currentRoles));
        if (!addResult.Succeeded)
        {
            return Result.Error(new ErrorList(
                addResult.Errors.Select(e => e.Description).ToList(),
                null));
        }

        return Result.Success();
    }
}
