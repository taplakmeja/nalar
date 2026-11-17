namespace Psikotes.Application.UseCases.Generators.Dtos;

public sealed record PreviewQuestionDto(
    int Difficulty,
    long Seed,
    string Prompt,
    string CorrectAnswer,
    IReadOnlyList<string> Options);
