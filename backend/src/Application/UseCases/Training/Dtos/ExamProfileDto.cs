namespace Psikotes.Application.UseCases.Training.Dtos;

public sealed record ExamProfileDto(
    Guid Id,
    string Slug,
    string Name,
    string Description,
    int QuestionCount,
    int DurationMinutes,
    decimal PassingScorePercent);
