using Psikotes.QuestionEngine.Abstractions;
using Psikotes.QuestionEngine.Models;
using Psikotes.QuestionEngine.Random;

namespace Psikotes.QuestionEngine.Generators;

/// <summary>
/// Sample generator proving the engine contract: deterministic, seeded,
/// with automatic answer and distractor generation.
/// </summary>
public sealed class ArithmeticAdditionGenerator : IQuestionGenerator
{
    public GeneratorIdentity Identity { get; } = new("numerical.addition", 1, QuestionCategory.Numerical);

    private static readonly int[] DifficultyRanges =
    {
        10, 50, 500, 5_000, 100_000
    };

    public GeneratedQuestion Generate(Difficulty difficulty, long seed)
    {
        var random = new SeededRandom(seed);
        var bound = DifficultyRanges[(int)difficulty];
        var a = random.Next(1, bound);
        var b = random.Next(1, bound);
        var correct = a + b;

        var distractors = new List<string>();
        foreach (var delta in new[] { 1, 10, 11 })
        {
            var candidate = correct + (random.Next(2) == 0 ? delta : -delta);
            if (candidate > 0 && candidate != correct && distractors.All(d => d != candidate.ToString()))
            {
                distractors.Add(candidate.ToString());
            }
        }

        while (distractors.Count < 3)
        {
            var candidate = correct + random.Next(1, 20);
            if (candidate != correct && distractors.All(d => d != candidate.ToString()))
            {
                distractors.Add(candidate.ToString());
            }
        }

        var options = random.Shuffle([correct.ToString(), .. distractors]);

        return new GeneratedQuestion(
            Identity,
            difficulty,
            seed,
            new Dictionary<string, string> { ["a"] = a.ToString(), ["b"] = b.ToString() },
            $"Berapakah hasil dari {a} + {b}?",
            correct.ToString(),
            options,
            ValidationStatus.Valid);
    }
}
