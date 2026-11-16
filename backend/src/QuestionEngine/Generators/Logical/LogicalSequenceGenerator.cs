using Psikotes.QuestionEngine.Abstractions;
using Psikotes.QuestionEngine.Models;
using Psikotes.QuestionEngine.Random;

namespace Psikotes.QuestionEngine.Generators.Logical;

public sealed class LogicalSequenceGenerator : IQuestionGenerator
{
    private static readonly string[] Days = { "Senin", "Selasa", "Rabu", "Kamis", "Jumat", "Sabtu", "Minggu" };

    public GeneratorIdentity Identity { get; } = new("logical.sequence", 1, QuestionCategory.Logical);

    public GeneratedQuestion Generate(Difficulty difficulty, long seed)
    {
        var random = new SeededRandom(seed);
        var todayIndex = random.Next(Days.Length);
        var offset = difficulty switch
        {
            Difficulty.Beginner => random.Next(2, 8),
            Difficulty.Easy => random.Next(8, 20),
            Difficulty.Intermediate => random.Next(20, 60),
            Difficulty.Advanced => random.Next(60, 200),
            _ => random.Next(200, 900)
        };

        var answer = Days[(todayIndex + offset) % Days.Length];
        var distractors = Days.Where(d => d != answer).OrderBy(_ => random.Next(7)).Take(3).ToList();
        var options = random.Shuffle([answer, .. distractors]);

        return new GeneratedQuestion(
            Identity,
            difficulty,
            seed,
            new Dictionary<string, string> { ["today"] = Days[todayIndex], ["offset"] = offset.ToString() },
            $"Jika hari ini hari {Days[todayIndex]}, maka {offset} hari kemudian jatuh pada hari apa?",
            answer,
            options,
            ValidationStatus.Valid);
    }
}
