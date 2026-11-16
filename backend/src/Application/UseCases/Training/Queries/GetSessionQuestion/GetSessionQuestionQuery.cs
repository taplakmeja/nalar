using Ardalis.Result;
using Mediator;
using Psikotes.Application.UseCases.Training.Dtos;

namespace Psikotes.Application.UseCases.Training.Queries.GetSessionQuestion;

public sealed record GetSessionQuestionQuery(Guid SessionId, int Order) : IQuery<Result<QuestionDto>>;
