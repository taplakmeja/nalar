namespace Psikotes.Application.UseCases.Analytics.Dtos;

public sealed record CategoryStatDto(
    string Category,
    int Answered,
    int Correct,
    double AccuracyPercent,
    int ConsecutiveCorrect,
    int ConsecutiveIncorrect);
