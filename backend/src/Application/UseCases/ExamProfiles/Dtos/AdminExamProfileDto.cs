namespace Psikotes.Application.UseCases.ExamProfiles.Dtos;

public sealed record AdminExamProfileDto(
    Guid Id,
    string Slug,
    string Name,
    string Description,
    string CategoryWeightsJson,
    int QuestionCount,
    int DurationMinutes,
    decimal PassingScorePercent,
    bool IsPublic);
