using Psikotes.QuestionEngine.Abstractions;
using Psikotes.QuestionEngine.Models;
using Psikotes.QuestionEngine.Random;
using Psikotes.QuestionEngine.Rendering;

namespace Psikotes.QuestionEngine.Generators.Spatial;

public sealed class CubeCountGenerator : IQuestionGenerator
{
    public GeneratorIdentity Identity { get; } = new("spatial.cube-count", 1, QuestionCategory.Spatial);

    public GeneratedQuestion Generate(Difficulty difficulty, long seed)
    {
        var random = new SeededRandom(seed);
        var columns = difficulty switch
        {
            Difficulty.Beginner => 3,
            Difficulty.Easy => random.Pick([3, 4]),
            Difficulty.Intermediate => random.Pick([4, 5]),
            Difficulty.Advanced => random.Pick([5, 6]),
            _ => random.Pick([6, 7])
        };

        var maxHeight = difficulty switch
        {
            Difficulty.Beginner => 3,
            Difficulty.Easy => 4,
            Difficulty.Intermediate => 5,
            Difficulty.Advanced => 6,
            _ => 7
        };

        var heights = Enumerable.Range(0, columns)
            .Select(_ => random.Next(1, maxHeight + 1))
            .ToList();
        var total = heights.Sum();
        var svg = SvgBuilder.CubeStack(heights);

        var distractors = NumericDistractors.CreateStrings(total, 3, random, maxOffset: System.Math.Max(2, columns));
        var options = random.Shuffle([total.ToString(), .. distractors]);

        return new GeneratedQuestion(
            Identity,
            difficulty,
            seed,
            new Dictionary<string, string> { ["heights"] = string.Join(",", heights) },
            "Berapa banyak kubus yang menyusun bangun berikut?",
            total.ToString(),
            options,
            ValidationStatus.Valid,
            Svg: svg);
    }
}
