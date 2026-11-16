using System.Text.Json;

namespace Psikotes.QuestionEngine.Datasets;

/// <summary>
/// Loads the versioned Indonesian verbal reference dataset embedded in the engine.
/// </summary>
public static class VerbalDatasetProvider
{
    public const string ResourceName = "Psikotes.QuestionEngine.Datasets.verbal-id-v1.json";

    private static readonly Lazy<VerbalDataset> _dataset = new(Load);

    public static VerbalDataset Dataset => _dataset.Value;

    private static VerbalDataset Load()
    {
        var assembly = typeof(VerbalDatasetProvider).Assembly;
        using var stream = assembly.GetManifestResourceStream(ResourceName)
            ?? throw new InvalidOperationException($"Embedded dataset '{ResourceName}' not found.");

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var dto = JsonSerializer.Deserialize<DatasetDto>(stream, options)
            ?? throw new InvalidOperationException("Verbal dataset could not be deserialized.");

        return new VerbalDataset(
            dto.Version,
            dto.Synonyms,
            dto.Antonyms,
            dto.Analogies,
            dto.Classifications);
    }

    private sealed record DatasetDto(
        int Version,
        IReadOnlyList<WordPair> Synonyms,
        IReadOnlyList<WordPair> Antonyms,
        IReadOnlyList<AnalogyEntry> Analogies,
        IReadOnlyList<ClassificationEntry> Classifications);
}
