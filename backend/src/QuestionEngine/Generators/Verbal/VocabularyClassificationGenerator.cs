using Psikotes.QuestionEngine.Abstractions;
using Psikotes.QuestionEngine.Datasets;
using Psikotes.QuestionEngine.Models;
using Psikotes.QuestionEngine.Random;

namespace Psikotes.QuestionEngine.Generators.Verbal;

public sealed class VocabularyClassificationGenerator : IQuestionGenerator
{
    public GeneratorIdentity Identity { get; } = new("verbal.classification", 1, QuestionCategory.Verbal);

    public GeneratedQuestion Generate(Difficulty difficulty, long seed)
    {
        var random = new SeededRandom(seed);
        var dataset = VerbalDatasetProvider.Dataset;
        var group = random.Pick(dataset.Classifications);

        var member = random.Pick(group.Members);
        var outsiders = random.Shuffle(group.Outsiders).Take(3).ToList();

        var distractors = outsiders;
        var fallback = 0;
        while (distractors.Count < 3)
        {
            var other = dataset.Classifications
                .Where(c => c.Category != group.Category)
                .SelectMany(c => c.Members)
                .Where(m => m != member && !distractors.Contains(m))
                .First();
            distractors.Add(other);
            fallback++;
            if (fallback > 10) break;
        }

        var options = random.Shuffle([member, .. distractors.Distinct().Take(3)]);

        return new GeneratedQuestion(
            Identity,
            difficulty,
            seed,
            new Dictionary<string, string> { ["category"] = group.Category, ["datasetVersion"] = dataset.Version.ToString() },
            $"Manakah di antara kata berikut yang TIDAK termasuk dalam kelompok {group.Category.ToLowerInvariant()}?",
            outsiders[0],
            options,
            ValidationStatus.Valid);
    }
}
