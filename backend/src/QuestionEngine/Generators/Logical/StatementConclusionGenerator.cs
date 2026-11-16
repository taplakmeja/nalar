using Psikotes.QuestionEngine.Abstractions;
using Psikotes.QuestionEngine.Models;
using Psikotes.QuestionEngine.Random;

namespace Psikotes.QuestionEngine.Generators.Logical;

public sealed class StatementConclusionGenerator : IQuestionGenerator
{
    private static readonly (string Antecedent, string Consequent)[] Rules =
    {
        ("hujan turun", "jalan basah"),
        ("siswa belajar sungguh-sungguh", "nilainya membaik"),
        ("mesin tidak dirawat", "mesin cepat rusak"),
        ("harga naik", "permintaan turun")
    };

    public GeneratorIdentity Identity { get; } = new("logical.conclusion", 1, QuestionCategory.Logical);

    public GeneratedQuestion Generate(Difficulty difficulty, long seed)
    {
        var random = new SeededRandom(seed);
        var rule = random.Pick(Rules);
        var variant = random.Next(3);

        (string prompt, string correct, string[] wrong) = variant switch
        {
            0 => (
                $"Jika {rule.Antecedent}, maka {rule.Consequent}. {Capital(rule.Antecedent)} tidak terjadi. Apa kesimpulannya?",
                $"Tidak dapat ditarik kesimpulan pasti tentang apakah {rule.Consequent}",
                new[]
                {
                    $"Pasti {rule.Consequent}",
                    $"Pasti {rule.Consequent} tidak terjadi"
                }),
            1 => (
                $"Jika {rule.Antecedent}, maka {rule.Consequent}. {Capital(rule.Consequent)} ternyata tidak terjadi. Apa kesimpulannya?",
                $"Pasti {rule.Antecedent} tidak terjadi",
                new[]
                {
                    $"Pasti {rule.Antecedent} terjadi",
                    $"Tidak dapat ditarik kesimpulan apa pun"
                }),
            _ => (
                $"Jika {rule.Antecedent}, maka {rule.Consequent}. {Capital(rule.Antecedent)} terjadi. Apa kesimpulannya?",
                $"Pasti {rule.Consequent}",
                new[]
                {
                    $"Pasti {rule.Consequent} tidak terjadi",
                    $"Tidak dapat ditarik kesimpulan apa pun"
                })
        };

        var wrongOptions = wrong.Distinct().Where(w => w != correct).Take(3).ToList();
        var options = random.Shuffle([correct, .. wrongOptions]);

        return new GeneratedQuestion(
            Identity,
            difficulty,
            seed,
            new Dictionary<string, string> { ["variant"] = variant.ToString() },
            prompt,
            correct,
            options,
            ValidationStatus.Valid);
    }

    private static string Capital(string text) =>
        string.Concat(char.ToUpperInvariant(text[0]), text[1..]);
}
