using Psikotes.QuestionEngine.Abstractions;

namespace Psikotes.QuestionEngine.Models;

public sealed record GeneratorIdentity(string Id, int Version, QuestionCategory Category);

public sealed record GeneratedQuestion(
    GeneratorIdentity Generator,
    Difficulty Difficulty,
    long Seed,
    IReadOnlyDictionary<string, string> Parameters,
    string Prompt,
    string CorrectAnswer,
    IReadOnlyList<string> Options,
    ValidationStatus Validation);

public interface IQuestionGenerator
{
    GeneratorIdentity Identity { get; }

    GeneratedQuestion Generate(Difficulty difficulty, long seed);
}
