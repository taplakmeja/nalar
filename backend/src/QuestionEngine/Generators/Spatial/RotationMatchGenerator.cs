using Psikotes.QuestionEngine.Abstractions;
using Psikotes.QuestionEngine.Models;
using Psikotes.QuestionEngine.Random;
using Psikotes.QuestionEngine.Rendering;

namespace Psikotes.QuestionEngine.Generators.Spatial;

public sealed class RotationMatchGenerator : IQuestionGenerator
{
    private static readonly (int X, int Y)[][] Shapes =
    {
        [(10, 10), (60, 10), (60, 30), (30, 30), (30, 60), (10, 60)],
        [(10, 20), (50, 10), (70, 40), (40, 70), (10, 50)]
    };

    public GeneratorIdentity Identity { get; } = new("spatial.rotation-match", 1, QuestionCategory.Spatial);

    public GeneratedQuestion Generate(Difficulty difficulty, long seed)
    {
        var random = new SeededRandom(seed);
        var shape = random.Pick(Shapes);
        var fill = "#6366f1";
        var rotation = random.Pick([90, 180, 270]);

        var questionSvg = SvgBuilder.Wrap(SvgBuilder.Polygon(shape, fill, rotation), 90);

        var correct = SvgBuilder.Wrap(SvgBuilder.Polygon(shape, "#6366f1", (rotation + 90) % 360), 90);

        var mirroredPoints = shape.Select(p => (X: 80 - p.X, Y: p.Y)).ToList();
        var distractors = new List<string>
        {
            SvgBuilder.Wrap(SvgBuilder.Polygon(mirroredPoints, "#6366f1", rotation), 90),
            SvgBuilder.Wrap(SvgBuilder.Polygon(mirroredPoints, "#6366f1", (rotation + 90) % 360), 90),
            SvgBuilder.Wrap(SvgBuilder.Polygon(mirroredPoints, "#6366f1", (rotation + 180) % 360), 90)
        };

        var options = random.Shuffle([correct, .. distractors]);

        return new GeneratedQuestion(
            Identity,
            difficulty,
            seed,
            new Dictionary<string, string> { ["rotation"] = rotation.ToString() },
            "Manakah pilihan yang merupakan bangun yang SAMA dengan acuan, hanya diputar (bukan dicerminkan)?",
            correct,
            options,
            ValidationStatus.Valid,
            Svg: questionSvg);
    }
}
