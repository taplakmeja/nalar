using Psikotes.QuestionEngine.Abstractions;
using Psikotes.QuestionEngine.Models;
using Psikotes.QuestionEngine.Random;

namespace Psikotes.QuestionEngine.Generators.Spatial;

public sealed class PaperFoldingGenerator : IQuestionGenerator
{
    public GeneratorIdentity Identity { get; } = new("spatial.paper-folding", 1, QuestionCategory.Spatial);

    public GeneratedQuestion Generate(Difficulty difficulty, long seed)
    {
        var random = new SeededRandom(seed);
        var folds = difficulty switch
        {
            Difficulty.Beginner => 1,
            Difficulty.Easy => random.Pick([1, 2]),
            Difficulty.Intermediate => 2,
            Difficulty.Advanced => random.Pick([2, 3]),
            _ => 3
        };

        var holes = random.Next(1, 3);
        var answer = holes * (int)System.Math.Pow(2, folds);

        var distractors = NumericDistractors.CreateStrings(answer, 3, random, maxOffset: System.Math.Max(2, answer / 2));
        var options = random.Shuffle([answer.ToString(), .. distractors]);

        return new GeneratedQuestion(
            Identity,
            difficulty,
            seed,
            new Dictionary<string, string> { ["folds"] = folds.ToString(), ["holes"] = holes.ToString() },
            $"Selembar kertas dilipat menjadi {folds} kali, kemudian dilubangi pada {holes} tempat. Berapa banyak lubang pada kertas setelah dibuka kembali?",
            answer.ToString(),
            options,
            ValidationStatus.Valid);
    }
}
