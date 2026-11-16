using Psikotes.QuestionEngine.Abstractions;
using Psikotes.QuestionEngine.Models;
using Psikotes.QuestionEngine.Random;

namespace Psikotes.QuestionEngine.Generators.Numerical;

public sealed class NumberSequenceGenerator : IQuestionGenerator
{
    public GeneratorIdentity Identity { get; } = new("numerical.sequence", 1, QuestionCategory.Numerical);

    public GeneratedQuestion Generate(Difficulty difficulty, long seed)
    {
        var random = new SeededRandom(seed);
        var pattern = random.Next(4);
        var terms = new List<int>();
        int step;
        int next;

        switch (pattern)
        {
            case 0:
                step = Step(difficulty, random);
                terms.Add(random.Next(1, 10));
                for (var i = 0; i < 4; i++)
                {
                    terms.Add(terms[^1] + step);
                }
                next = terms[^1] + step;
                break;
            case 1:
                var ratio = difficulty switch
                {
                    Difficulty.Beginner => 2,
                    Difficulty.Easy => random.Pick([2, 3]),
                    Difficulty.Intermediate => random.Pick([2, 3, 4]),
                    _ => random.Pick([3, 4, 5])
                };
                terms.Add(random.Next(1, 4));
                for (var i = 0; i < 4; i++)
                {
                    terms.Add(terms[^1] * ratio);
                }
                next = terms[^1] * ratio;
                break;
            case 2:
                var up = Step(difficulty, random);
                var down = Step(difficulty, random);
                terms.Add(random.Next(10, 30));
                for (var i = 0; i < 5; i++)
                {
                    terms.Add(i % 2 == 0 ? terms[^1] + up : System.Math.Max(1, terms[^1] - down));
                }
                next = terms[^1] + up;
                break;
            default:
                var start = random.Next(1, 4);
                terms = Enumerable.Range(start, 5).Select(n => n * n).ToList();
                next = (start + 5) * (start + 5);
                break;
        }

        var shown = string.Join(", ", terms);
        var distractors = NumericDistractors.CreateStrings(next, 3, random, maxOffset: System.Math.Max(4, next / 5));
        var options = random.Shuffle([next.ToString(), .. distractors]);

        return new GeneratedQuestion(
            Identity,
            difficulty,
            seed,
            new Dictionary<string, string> { ["pattern"] = pattern.ToString(), ["terms"] = shown },
            $"Lanjutkan pola berikut: {shown}, ...",
            next.ToString(),
            options,
            ValidationStatus.Valid);
    }

    private static int Step(Difficulty difficulty, SeededRandom random) => difficulty switch
    {
        Difficulty.Beginner => random.Next(1, 4),
        Difficulty.Easy => random.Next(2, 7),
        Difficulty.Intermediate => random.Next(3, 12),
        Difficulty.Advanced => random.Next(5, 20),
        _ => random.Next(10, 40)
    };
}
