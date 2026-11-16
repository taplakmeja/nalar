using Ardalis.Result;
using Mediator;

namespace Psikotes.Application.UseCases.Auth.Commands.Refresh;

public sealed record RefreshCommand(string RefreshToken) : ICommand<Result<AuthResponse>>;
