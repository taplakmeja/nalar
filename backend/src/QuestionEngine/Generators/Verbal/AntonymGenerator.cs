using Psikotes.QuestionEngine.Abstractions;
using Psikotes.QuestionEngine.Datasets;
using Psikotes.QuestionEngine.Models;
using Psikotes.QuestionEngine.Random;

namespace Psikotes.QuestionEngine.Generators.Verbal;

public sealed class AntonymGenerator : IQuestionGenerator
{
    public GeneratorIdentity Identity { get; } = new("verbal.antonym", 1, QuestionCategory.Verbal);

    public GeneratedQuestion Generate(Difficulty difficulty, long seed)
    {
        var random = new SeededRandom(seed);
        var dataset = VerbalDatasetProvider.Dataset;
        var entry = random.Pick(dataset.Antonyms);
        var others = dataset.Antonyms
            .Where(p => p.Match != entry.Match && p.Word != entry.Word)
            .Select(p => p.Match)
            .Distinct()
            .ToList();

        var distractors = random.Shuffle(others).Take(3).ToList();
        var fallback = 0;
        while (distractors.Count < 3)
        {
            distractors.Add($"{entry.Match}-alternatif{++fallback}");
        }

        var options = random.Shuffle([entry.Match, .. distractors]);

        return new GeneratedQuestion(
            Identity,
            difficulty,
            seed,
            new Dictionary<string, string> { ["word"] = entry.Word, ["datasetVersion"] = dataset.Version.ToString() },
            $"Pilihlah kata yang merupakan lawan (antonim) dari kata: {entry.Word.ToUpperInvariant()}",
            entry.Match,
            options,
            ValidationStatus.Valid);
    }
}
