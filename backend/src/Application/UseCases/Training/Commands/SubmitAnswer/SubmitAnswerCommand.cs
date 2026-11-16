using Ardalis.Result;
using Mediator;
using Psikotes.Application.UseCases.Training.Dtos;

namespace Psikotes.Application.UseCases.Training.Commands.SubmitAnswer;

public sealed record SubmitAnswerCommand(Guid SessionId, Guid QuestionId, string Answer) : ICommand<Result<AnswerFeedback>>;
