using Ardalis.Result;
using Mediator;

namespace Psikotes.Application.UseCases.Admin.Users;

public sealed record SetUserRolesCommand(string UserId, IReadOnlyList<string> Roles) : ICommand<Result>;
