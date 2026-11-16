using Ardalis.Result;
using Mediator;

namespace Psikotes.Application.UseCases.Auth.Commands.Register;

public sealed record RegisterCommand(string Email, string Password, string DisplayName)
    : ICommand<Result<AuthResponse>>;
