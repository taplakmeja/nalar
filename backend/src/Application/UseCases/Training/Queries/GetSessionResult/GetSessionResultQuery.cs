using Ardalis.Result;
using Mediator;
using Psikotes.Application.UseCases.Training.Dtos;

namespace Psikotes.Application.UseCases.Training.Queries.GetSessionResult;

public sealed record GetSessionResultQuery(Guid SessionId) : IQuery<Result<SessionResultDto>>;
