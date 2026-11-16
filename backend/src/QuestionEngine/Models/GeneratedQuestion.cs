using Psikotes.QuestionEngine.Abstractions;

namespace Psikotes.QuestionEngine.Models;

public sealed record GeneratedQuestion(
    GeneratorIdentity Generator,
    Difficulty Difficulty,
    long Seed,
    IReadOnlyDictionary<string, string> Parameters,
    string Prompt,
    string CorrectAnswer,
    IReadOnlyList<string> Options,
    ValidationStatus Validation,
    string? Svg = null);
