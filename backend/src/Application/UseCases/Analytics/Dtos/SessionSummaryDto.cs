namespace Psikotes.Application.UseCases.Analytics.Dtos;

public sealed record SessionSummaryDto(
    Guid SessionId,
    string Mode,
    string Status,
    int TotalQuestions,
    int CorrectCount,
    int IncorrectCount,
    int UnansweredCount,
    decimal Score,
    DateTime? CompletedAtUtc);
