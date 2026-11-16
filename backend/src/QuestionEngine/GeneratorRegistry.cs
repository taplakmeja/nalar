using Psikotes.QuestionEngine.Abstractions;
using Psikotes.QuestionEngine.Models;

namespace Psikotes.QuestionEngine;

public sealed class GeneratorRegistry(IEnumerable<IQuestionGenerator> generators)
{
    private readonly Dictionary<string, IQuestionGenerator> _generators =
        generators.ToDictionary(g => $"{g.Identity.Id}@v{g.Identity.Version}", StringComparer.OrdinalIgnoreCase);

    public IReadOnlyCollection<IQuestionGenerator> All => _generators.Values;

    public IQuestionGenerator? Get(string generatorId, int version) =>
        _generators.TryGetValue($"{generatorId}@v{version}", out var generator) ? generator : null;

    public IQuestionGenerator? GetLatest(string generatorId) =>
        _generators.Values
            .Where(g => g.Identity.Id.Equals(generatorId, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(g => g.Identity.Version)
            .FirstOrDefault();
}
