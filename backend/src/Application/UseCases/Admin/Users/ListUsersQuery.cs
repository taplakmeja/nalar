using Ardalis.Result;
using Mediator;

namespace Psikotes.Application.UseCases.Admin.Users;

public sealed record AdminUserDto(string Id, string Email, string DisplayName, IReadOnlyList<string> Roles, bool Locked);

public sealed record ListUsersQuery : IQuery<Result<IReadOnlyList<AdminUserDto>>>;
