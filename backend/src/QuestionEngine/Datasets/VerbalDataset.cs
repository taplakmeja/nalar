namespace Psikotes.QuestionEngine.Datasets;

public sealed record VerbalDataset(
    int Version,
    IReadOnlyList<WordPair> Synonyms,
    IReadOnlyList<WordPair> Antonyms,
    IReadOnlyList<AnalogyEntry> Analogies,
    IReadOnlyList<ClassificationEntry> Classifications);

public sealed record WordPair(string Word, string Match);

public sealed record AnalogyEntry(string A, string B, string C, string D);

public sealed record ClassificationEntry(
    string Category,
    IReadOnlyList<string> Members,
    IReadOnlyList<string> Outsiders);
