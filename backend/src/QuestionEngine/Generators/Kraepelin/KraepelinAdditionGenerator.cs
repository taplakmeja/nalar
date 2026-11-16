using Psikotes.QuestionEngine.Abstractions;
using Psikotes.QuestionEngine.Models;
using Psikotes.QuestionEngine.Random;

namespace Psikotes.QuestionEngine.Generators.Kraepelin;

/// <summary>
/// Kraepelin/Pauli-style continuous single-digit addition drill, expressed as
/// an MCQ so it fits the standard session pipeline with time pressure.
/// </summary>
public sealed class KraepelinAdditionGenerator : IQuestionGenerator
{
    public GeneratorIdentity Identity { get; } = new("kraepelin.addition", 1, QuestionCategory.Kraepelin);

    public GeneratedQuestion Generate(Difficulty difficulty, long seed)
    {
        var random = new SeededRandom(seed);
        var a = random.Next(1, 10);
        var b = random.Next(1, 10);
        var correct = a + b;

        var distractors = NumericDistractors.CreateStrings(correct, 3, random, maxOffset: 4);
        var options = random.Shuffle([correct.ToString(), .. distractors]);

        return new GeneratedQuestion(
            Identity,
            difficulty,
            seed,
            new Dictionary<string, string> { ["a"] = a.ToString(), ["b"] = b.ToString() },
            $"{a} + {b} = ?",
            correct.ToString(),
            options,
            ValidationStatus.Valid);
    }
}
