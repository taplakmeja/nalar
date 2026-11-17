using Ardalis.GuardClauses;
using Psikotes.Application.Common.Interfaces;
using Psikotes.QuestionEngine;
using Psikotes.QuestionEngine.Abstractions;
using Psikotes.QuestionEngine.Rules;

namespace Psikotes.Application.Common.Services;

public sealed class RuleGeneratorFactory : IRuleGeneratorFactory
{
    public RuleBasedGenerator Create(string generatorId, int version, string category, string configJson, out IReadOnlyList<string> issues)
    {
        Guard.Against.NullOrWhiteSpace(generatorId);
        Guard.Against.NullOrWhiteSpace(configJson);

        var definition = GeneratorConfigDefinition.Parse(configJson);
        issues = RuleConfigValidator.Validate(definition);

        if (!Enum.TryParse<QuestionCategory>(category, ignoreCase: true, out var parsedCategory))
        {
            issues = issues.Append($"Unknown category '{category}'.").ToList();
            parsedCategory = QuestionCategory.Numerical;
        }

        return new RuleBasedGenerator(generatorId, version, definition)
        {
            Category = parsedCategory
        };
    }
}
