using Ardalis.Result;
using Mediator;

namespace Psikotes.Application.UseCases.Auth.Commands.Logout;

public sealed record LogoutCommand(string RefreshToken) : ICommand<Result>;
