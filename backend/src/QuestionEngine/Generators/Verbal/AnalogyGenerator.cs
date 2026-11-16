using Psikotes.QuestionEngine.Abstractions;
using Psikotes.QuestionEngine.Datasets;
using Psikotes.QuestionEngine.Models;
using Psikotes.QuestionEngine.Random;

namespace Psikotes.QuestionEngine.Generators.Verbal;

public sealed class AnalogyGenerator : IQuestionGenerator
{
    public GeneratorIdentity Identity { get; } = new("verbal.analogy", 1, QuestionCategory.Verbal);

    public GeneratedQuestion Generate(Difficulty difficulty, long seed)
    {
        var random = new SeededRandom(seed);
        var dataset = VerbalDatasetProvider.Dataset;
        var entry = random.Pick(dataset.Analogies);
        var others = dataset.Analogies
            .Where(a => a.D != entry.D)
            .Select(a => a.D)
            .Distinct()
            .ToList();

        var distractors = random.Shuffle(others).Take(3).ToList();
        var fallback = 0;
        while (distractors.Count < 3)
        {
            distractors.Add($"{entry.D}-alternatif{++fallback}");
        }

        var options = random.Shuffle([entry.D, .. distractors]);

        return new GeneratedQuestion(
            Identity,
            difficulty,
            seed,
            new Dictionary<string, string> { ["pair"] = $"{entry.A}-{entry.C}", ["datasetVersion"] = dataset.Version.ToString() },
            $"{entry.A.ToUpperInvariant()} : {entry.B} = {entry.C.ToUpperInvariant()} : ...",
            entry.D,
            options,
            ValidationStatus.Valid);
    }
}
