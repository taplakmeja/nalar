namespace Psikotes.Application.UseCases.Training.Dtos;

public sealed record SessionQuestionResultDto(
    int Order,
    string Category,
    string GeneratorId,
    int Difficulty,
    bool IsCorrect,
    string? SubmittedAnswer,
    string CorrectAnswer);
