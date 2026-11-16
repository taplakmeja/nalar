using Psikotes.QuestionEngine.Abstractions;
using Psikotes.QuestionEngine.Models;
using Psikotes.QuestionEngine.Random;
using Psikotes.QuestionEngine.Rendering;

namespace Psikotes.QuestionEngine.Generators.Abstract;

public sealed class MissingFigureGenerator : IQuestionGenerator
{
    private static readonly string[] Fills = { "#8b5cf6", "#0ea5e9", "#f97316", "#14b8a6" };

    public GeneratorIdentity Identity { get; } = new("abstract.missing-figure", 1, QuestionCategory.Abstract);

    public GeneratedQuestion Generate(Difficulty difficulty, long seed)
    {
        var random = new SeededRandom(seed);
        var fill = Fills[random.Next(Fills.Length)];
        var countStep = difficulty switch
        {
            Difficulty.Beginner => 1,
            Difficulty.Easy => random.Pick([1, 2]),
            Difficulty.Intermediate => random.Pick([2, 3]),
            Difficulty.Advanced => random.Pick([3, 4]),
            _ => random.Pick([4, 5])
        };

        string Figure(int dots) => SvgBuilder.Wrap(
            string.Join(string.Empty, Enumerable.Range(0, dots).Select(i =>
                SvgBuilder.Circle(fill, cx: 20 + (i % 4) * 14, cy: i < 4 ? 40 : 20, r: 5))),
            80);

        var counts = new[] { countStep, countStep * 2, countStep * 3, countStep * 4 };
        var missingIndex = random.Next(counts.Length);
        var missingValue = counts[missingIndex];

        var sequenceSvg = string.Join(
            string.Empty,
            counts.Select((c, i) => i == missingIndex
                ? SvgBuilder.Wrap($"<text x=\"40\" y=\"50\" text-anchor=\"middle\" font-size=\"28\">?</text>", 80)
                : Figure(c)));

        var correct = Figure(missingValue);
        var wrongValues = new[] { missingValue + countStep, missingValue - countStep, missingValue + 2 * countStep }
            .Where(v => v > 0 && v != missingValue)
            .Distinct()
            .ToList();

        var distractors = new List<string>();
        foreach (var value in wrongValues)
        {
            if (distractors.Count >= 3) break;
            var figure = Figure(value);
            if (figure != correct && !distractors.Contains(figure))
            {
                distractors.Add(figure);
            }
        }

        var fallback = 1;
        while (distractors.Count < 3)
        {
            var figure = Figure(missingValue + countStep * 2 + fallback * 7);
            if (figure != correct && !distractors.Contains(figure))
            {
                distractors.Add(figure);
            }
            fallback++;
        }

        var options = random.Shuffle([correct, .. distractors]);

        return new GeneratedQuestion(
            Identity,
            difficulty,
            seed,
            new Dictionary<string, string> { ["step"] = countStep.ToString(), ["missingValue"] = missingValue.ToString() },
            "Manakah gambar yang hilang dari pola berikut?",
            correct,
            options,
            ValidationStatus.Valid,
            Svg: sequenceSvg);
    }
}
