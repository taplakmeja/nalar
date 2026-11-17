namespace Psikotes.Application.Common.Models;

public sealed record AnsweredQuestionEvent(
    string Category,
    string GeneratorId,
    int Difficulty,
    bool IsCorrect,
    DateTime AnsweredAtUtc);
