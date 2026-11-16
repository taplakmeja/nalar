using Ardalis.Result;
using Mediator;

namespace Psikotes.Application.UseCases.Auth.Commands.Login;

public sealed record LoginCommand(string Email, string Password) : ICommand<Result<AuthResponse>>;
