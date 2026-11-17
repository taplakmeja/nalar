using Psikotes.QuestionEngine.Rules;

namespace Psikotes.Application.Common.Interfaces;

public interface IRuleGeneratorFactory
{
    RuleBasedGenerator Create(string generatorId, int version, string category, string configJson, out IReadOnlyList<string> issues);
}
