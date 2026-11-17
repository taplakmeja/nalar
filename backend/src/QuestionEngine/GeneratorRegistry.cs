using Psikotes.QuestionEngine.Abstractions;
using Psikotes.QuestionEngine.Models;

namespace Psikotes.QuestionEngine;

/// <summary>
/// Holds every known generator version for deterministic regeneration
/// (Get/GetLatest) and the subset of versions eligible for question
/// selection (All — the "active" set).
/// </summary>
public sealed class GeneratorRegistry
{
    private readonly Dictionary<string, IQuestionGenerator> _versions =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly Dictionary<string, IQuestionGenerator> _active =
        new(StringComparer.OrdinalIgnoreCase);

    public GeneratorRegistry(IEnumerable<IQuestionGenerator> generators)
    {
        foreach (var generator in generators)
        {
            Activate(generator);
        }
    }

    public GeneratorRegistry()
        : this([])
    {
    }

    public IReadOnlyCollection<IQuestionGenerator> All => _active.Values;

    public IQuestionGenerator? Get(string generatorId, int version) =>
        _versions.TryGetValue($"{generatorId}@v{version}", out var generator) ? generator : null;

    public IQuestionGenerator? GetLatest(string generatorId) =>
        _versions.Values
            .Where(g => g.Identity.Id.Equals(generatorId, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(g => g.Identity.Version)
            .FirstOrDefault();

    public void Add(IQuestionGenerator generator) =>
        _versions[$"{generator.Identity.Id}@v{generator.Identity.Version}"] = generator;

    public void Activate(IQuestionGenerator generator)
    {
        Add(generator);
        _active[generator.Identity.Id] = generator;
    }

    public void Deactivate(string generatorId) =>
        _active.Remove(generatorId);
}
