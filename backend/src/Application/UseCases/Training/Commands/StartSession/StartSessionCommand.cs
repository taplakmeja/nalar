using Ardalis.Result;
using Mediator;
using Psikotes.Application.UseCases.Training.Dtos;

namespace Psikotes.Application.UseCases.Training.Commands.StartSession;

public sealed record StartSessionCommand(
    string Mode,
    string CategoryScope,
    int Difficulty,
    int QuestionCount,
    string? ExamProfileSlug,
    bool UseAdaptiveDifficulty = false) : ICommand<Result<StartSessionResponse>>;
