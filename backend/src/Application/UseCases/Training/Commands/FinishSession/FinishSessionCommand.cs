using Ardalis.Result;
using Mediator;
using Psikotes.Application.UseCases.Training.Dtos;

namespace Psikotes.Application.UseCases.Training.Commands.FinishSession;

public sealed record FinishSessionCommand(Guid SessionId) : ICommand<Result<SessionResultDto>>;
