namespace Psikotes.Application.UseCases.Analytics.Dtos;

public sealed record DifficultyStatDto(
    int Difficulty,
    int Answered,
    int Correct,
    double AccuracyPercent);
