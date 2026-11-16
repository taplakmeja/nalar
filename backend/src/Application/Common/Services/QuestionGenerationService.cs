using Ardalis.GuardClauses;
using Psikotes.Application.Common.Interfaces;
using Psikotes.Domain.Enums;
using Psikotes.QuestionEngine;
using Psikotes.QuestionEngine.Abstractions;
using Psikotes.QuestionEngine.Models;

namespace Psikotes.Application.Common.Services;

public class QuestionGenerationService : IQuestionGenerationService
{
    private readonly GeneratorRegistry _registry;

    public QuestionGenerationService(GeneratorRegistry registry)
    {
        Guard.Against.Null(registry);
        _registry = registry;
    }

    public GeneratedQuestion Generate(string category, int difficulty, long seed)
    {
        Guard.Against.NullOrWhiteSpace(category);
        var scope = Enum.Parse<QuestionCategoryScope>(category, ignoreCase: true);
        var generators = scope == QuestionCategoryScope.Mixed
            ? _registry.All.ToList()
            : _registry.All
                .Where(g => g.Identity.Category == Enum.Parse<QuestionCategory>(scope.ToString(), ignoreCase: true))
                .ToList();

        if (generators.Count == 0)
        {
            throw new InvalidOperationException($"No generators registered for category '{category}'.");
        }

        var generator = generators[(int)(seed % generators.Count)];
        return generator.Generate((Difficulty)difficulty, seed);
    }
}
