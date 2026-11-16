using Psikotes.QuestionEngine.Abstractions;
using Psikotes.QuestionEngine.Models;
using Psikotes.QuestionEngine.Random;

namespace Psikotes.QuestionEngine.Generators.Logical;

public sealed class SyllogismGenerator : IQuestionGenerator
{
    private static readonly string[][] Subjects =
    {
        ["guru", "pegawai"],
        ["dokter", "manusia"],
        ["siswa", "pembelajar"],
        ["penyair", "penulis"],
        ["insinyur", "lulusan universitas"]
    };

    private static readonly string[][] Predicates =
    {
        ["membaca buku", "menyukai ilmu"],
        ["bekerja keras", "mencapai tujuan"],
        ["mengikuti pelatihan", "meningkatkan kemampuan"],
        ["berlatih rutin", "menguasai materi"]
    };

    public GeneratorIdentity Identity { get; } = new("logical.syllogism", 1, QuestionCategory.Logical);

    public GeneratedQuestion Generate(Difficulty difficulty, long seed)
    {
        var random = new SeededRandom(seed);
        var subjects = random.Pick(Subjects);
        var predicates = random.Pick(Predicates);
        var a = subjects[0];
        var b = subjects[1];
        var p = predicates[0];
        var q = predicates[1];

        var correct = $"Semua {a} yang {p} juga {q}";
        var wrongOptions = new[]
        {
            $"Semua yang {q} adalah {a}",
            $"Tidak ada {a} yang {q}",
            $"Tidak ada {b} yang {p}"
        }.Where(o => o != correct).ToList();

        var options = random.Shuffle([correct, .. wrongOptions.Take(3)]);

        return new GeneratedQuestion(
            Identity,
            difficulty,
            seed,
            new Dictionary<string, string> { ["a"] = a, ["b"] = b },
            $"Semua {a} adalah {b}. Semua {b} sedang {p}. Manakah kesimpulan yang PASTI benar?",
            correct,
            options,
            ValidationStatus.Valid);
    }
}
