namespace Psikotes.Application.UseCases.Training.Dtos;

public sealed record QuestionDto(
    Guid QuestionId,
    int Order,
    string Category,
    int Difficulty,
    string Prompt,
    IReadOnlyList<string> Options,
    string? Svg);
