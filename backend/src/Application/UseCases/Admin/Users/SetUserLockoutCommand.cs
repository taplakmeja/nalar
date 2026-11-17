using Ardalis.Result;
using Mediator;

namespace Psikotes.Application.UseCases.Admin.Users;

public sealed record SetUserLockoutCommand(string UserId, bool Locked) : ICommand<Result>;
