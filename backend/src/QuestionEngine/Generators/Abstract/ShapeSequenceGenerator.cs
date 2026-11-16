using Psikotes.QuestionEngine.Abstractions;
using Psikotes.QuestionEngine.Models;
using Psikotes.QuestionEngine.Random;
using Psikotes.QuestionEngine.Rendering;

namespace Psikotes.QuestionEngine.Generators.Abstract;

public sealed class ShapeSequenceGenerator : IQuestionGenerator
{
    private static readonly string[] Fills = { "#ef4444", "#3b82f6", "#22c55e", "#eab308" };

    public GeneratorIdentity Identity { get; } = new("abstract.shape-sequence", 1, QuestionCategory.Abstract);

    public GeneratedQuestion Generate(Difficulty difficulty, long seed)
    {
        var random = new SeededRandom(seed);
        var fill = Fills[random.Next(Fills.Length)];
        var rotationStep = difficulty switch
        {
            Difficulty.Beginner => random.Pick([0, 45]),
            Difficulty.Easy => random.Pick([30, 45, 90]),
            Difficulty.Intermediate => random.Pick([15, 30, 60]),
            Difficulty.Advanced => random.Pick([10, 20, 75]),
            _ => random.Pick([5, 12, 105])
        };

        string Shape(int rotation) => SvgBuilder.Triangle(fill, rotation);

        var sequenceSvg = string.Join(
            string.Empty,
            Enumerable.Range(0, 4).Select(i => SvgBuilder.Wrap(Shape(i * rotationStep), 70)));

        var nextRotation = 4 * rotationStep;
        var correct = SvgBuilder.Wrap(Shape(nextRotation), 70);

        var wrongRotations = new[] { nextRotation + rotationStep, nextRotation - rotationStep, nextRotation + 2 * rotationStep }
            .Where(r => r != nextRotation)
            .Distinct()
            .ToList();

        var distractors = wrongRotations
            .Take(3)
            .Select(r => SvgBuilder.Wrap(Shape(r), 70))
            .ToList();

        while (distractors.Count < 3)
        {
            distractors.Add(SvgBuilder.Wrap(Shape(nextRotation + 180 + distractors.Count), 70));
        }

        var options = random.Shuffle([correct, .. distractors]);

        return new GeneratedQuestion(
            Identity,
            difficulty,
            seed,
            new Dictionary<string, string> { ["rotationStep"] = rotationStep.ToString() },
            "Perhatikan pola perputaran bangun berikut. Manakah bangun yang tepat untuk melanjutkan pola?",
            correct,
            options,
            ValidationStatus.Valid,
            Svg: sequenceSvg);
    }
}
