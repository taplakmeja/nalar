namespace Psikotes.Application.UseCases.Training.Dtos;

public sealed record SessionResultDto(
    Guid SessionId,
    string Mode,
    string Status,
    int TotalQuestions,
    int CorrectCount,
    int IncorrectCount,
    int UnansweredCount,
    decimal Score,
    bool IsPassed,
    DateTime? DeadlineUtc,
    IReadOnlyList<SessionQuestionResultDto> Questions);
