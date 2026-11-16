using Psikotes.QuestionEngine.Abstractions;
using Psikotes.QuestionEngine.Mathematics;
using Psikotes.QuestionEngine.Models;
using Psikotes.QuestionEngine.Random;

namespace Psikotes.QuestionEngine.Generators.Numerical;

public sealed class FractionGenerator : IQuestionGenerator
{
    public GeneratorIdentity Identity { get; } = new("numerical.fraction", 1, QuestionCategory.Numerical);

    public GeneratedQuestion Generate(Difficulty difficulty, long seed)
    {
        var random = new SeededRandom(seed);
        var maxDenominator = difficulty switch
        {
            Difficulty.Beginner => 4,
            Difficulty.Easy => 6,
            Difficulty.Intermediate => 10,
            Difficulty.Advanced => 12,
            _ => 15
        };

        var denominator = random.Next(2, maxDenominator);
        var numerator = random.Next(1, denominator);
        var denominator2 = random.Next(2, maxDenominator);
        var numerator2 = random.Next(1, denominator2);

        var sum = Fraction.Add(new Fraction(numerator, denominator), new Fraction(numerator2, denominator2));

        var distractors = new List<string>();
        var candidates = new[]
        {
            Fraction.Simplify(numerator + numerator2, System.Math.Max(denominator, denominator2)),
            new Fraction(sum.Numerator + 1, sum.Denominator),
            new Fraction(sum.Numerator, sum.Denominator + 1),
            new Fraction(numerator + numerator2, denominator + denominator2)
        };

        foreach (var candidate in candidates)
        {
            var text = candidate.ToString();
            if (text != sum.ToString() && candidate.Denominator > 0 && !distractors.Contains(text))
            {
                distractors.Add(text);
            }
        }

        var fallback = 1;
        while (distractors.Count < 3)
        {
            var candidate = new Fraction(sum.Numerator + fallback, sum.Denominator + fallback);
            var text = candidate.ToString();
            if (text != sum.ToString() && !distractors.Contains(text))
            {
                distractors.Add(text);
            }
            fallback++;
        }

        var options = random.Shuffle([sum.ToString(), .. distractors]);

        return new GeneratedQuestion(
            Identity,
            difficulty,
            seed,
            new Dictionary<string, string>
            {
                ["a"] = $"{numerator}/{denominator}",
                ["b"] = $"{numerator2}/{denominator2}"
            },
            $"Berapakah hasil dari {numerator}/{denominator} + {numerator2}/{denominator2} (bentuk paling sederhana)?",
            sum.ToString(),
            options,
            ValidationStatus.Valid);
    }
}
