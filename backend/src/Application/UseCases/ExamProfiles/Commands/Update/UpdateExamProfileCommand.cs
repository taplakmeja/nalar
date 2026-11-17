using Ardalis.Result;
using Mediator;
using Psikotes.Application.UseCases.ExamProfiles.Dtos;

namespace Psikotes.Application.UseCases.ExamProfiles.Commands.Update;

public sealed record UpdateExamProfileCommand(
    Guid Id,
    string Name,
    string Description,
    string CategoryWeightsJson,
    int QuestionCount,
    int DurationMinutes,
    decimal PassingScorePercent,
    bool IsPublic) : ICommand<Result<AdminExamProfileDto>>;
