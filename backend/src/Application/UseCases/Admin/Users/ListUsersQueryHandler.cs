using Ardalis.Result;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Psikotes.Domain.Entities;

namespace Psikotes.Application.UseCases.Admin.Users;

public sealed class ListUsersQueryHandler(UserManager<ApplicationUser> userManager)
    : IQueryHandler<ListUsersQuery, Result<IReadOnlyList<AdminUserDto>>>
{
    public async ValueTask<Result<IReadOnlyList<AdminUserDto>>> Handle(ListUsersQuery query, CancellationToken cancellationToken)
    {
        var users = userManager.Users.OrderBy(u => u.Email).ToList();
        var result = new List<AdminUserDto>();

        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            result.Add(new AdminUserDto(
                user.Id,
                user.Email ?? string.Empty,
                user.DisplayName,
                roles.OrderBy(r => r).ToList(),
                user.LockoutEnd is not null && user.LockoutEnd > DateTimeOffset.UtcNow));
        }

        return Result.Success<IReadOnlyList<AdminUserDto>>(result);
    }
}
