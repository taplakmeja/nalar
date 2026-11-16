using Psikotes.QuestionEngine.Abstractions;
using Psikotes.QuestionEngine.Models;
using Psikotes.QuestionEngine.Random;

namespace Psikotes.QuestionEngine.Generators.Numerical;

public sealed class PercentageGenerator : IQuestionGenerator
{
    public GeneratorIdentity Identity { get; } = new("numerical.percentage", 1, QuestionCategory.Numerical);

    private static readonly int[] NicePercentages = { 5, 10, 15, 20, 25, 40, 50, 60, 75, 80 };

    public GeneratedQuestion Generate(Difficulty difficulty, long seed)
    {
        var random = new SeededRandom(seed);
        var percentage = difficulty switch
        {
            Difficulty.Beginner => random.Pick([10, 25, 50, 75]),
            Difficulty.Easy => random.Pick([5, 20, 40, 60]),
            Difficulty.Intermediate => random.Pick([15, 35, 45, 65]),
            Difficulty.Advanced => random.Pick([12, 18, 35, 85]),
            _ => random.Next(1, 100)
        };

        var baseValue = difficulty switch
        {
            Difficulty.Beginner => random.Next(2, 10) * 100,
            Difficulty.Easy => random.Next(2, 20) * 20,
            Difficulty.Intermediate => random.Next(20, 90) * 10,
            Difficulty.Advanced => random.Next(100, 999),
            _ => random.Next(1000, 99999)
        };

        var result = baseValue * percentage / 100.0;
        var rounded = (int)System.Math.Round(result);
        var isExact = System.Math.Abs(result - rounded) < 0.001;
        if (!isExact)
        {
            baseValue = baseValue - (baseValue % 100);
            result = baseValue * percentage / 100.0;
            rounded = (int)System.Math.Round(result);
        }

        var distractors = NumericDistractors.CreateStrings(rounded, 3, random, maxOffset: System.Math.Max(5, rounded / 4));
        var options = random.Shuffle([rounded.ToString(), .. distractors]);

        return new GeneratedQuestion(
            Identity,
            difficulty,
            seed,
            new Dictionary<string, string> { ["p"] = percentage.ToString(), ["n"] = baseValue.ToString() },
            $"Berapakah {percentage}% dari {baseValue:N0}?".Replace(",", "."),
            rounded.ToString(),
            options,
            ValidationStatus.Valid);
    }
}
