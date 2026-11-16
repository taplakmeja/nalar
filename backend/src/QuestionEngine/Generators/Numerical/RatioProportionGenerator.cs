using Psikotes.QuestionEngine.Abstractions;
using Psikotes.QuestionEngine.Models;
using Psikotes.QuestionEngine.Random;

namespace Psikotes.QuestionEngine.Generators.Numerical;

public sealed class RatioProportionGenerator : IQuestionGenerator
{
    public GeneratorIdentity Identity { get; } = new("numerical.ratio", 1, QuestionCategory.Numerical);

    public GeneratedQuestion Generate(Difficulty difficulty, long seed)
    {
        var random = new SeededRandom(seed);
        var a = random.Next(1, difficulty switch
        {
            Difficulty.Beginner => 4,
            Difficulty.Easy => 6,
            Difficulty.Intermediate => 9,
            Difficulty.Advanced => 12,
            _ => 20
        });
        var multiplier = random.Next(2, difficulty switch
        {
            Difficulty.Beginner => 4,
            Difficulty.Easy => 6,
            Difficulty.Intermediate => 10,
            Difficulty.Advanced => 15,
            _ => 25
        });
        var b = a * multiplier;
        var c = random.Next(1, a + 1);

        var x = c * multiplier;

        var distractors = NumericDistractors.CreateStrings(x, 3, random, maxOffset: System.Math.Max(3, multiplier));
        var options = random.Shuffle([x.ToString(), .. distractors]);

        return new GeneratedQuestion(
            Identity,
            difficulty,
            seed,
            new Dictionary<string, string> { ["a"] = a.ToString(), ["b"] = b.ToString(), ["c"] = c.ToString() },
            $"Jika perbandingan {a} : {b} setara dengan {c} : x, berapakah nilai x?",
            x.ToString(),
            options,
            ValidationStatus.Valid);
    }
}
