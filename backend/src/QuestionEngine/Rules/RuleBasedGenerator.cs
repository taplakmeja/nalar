using System.Globalization;
using System.Text.RegularExpressions;
using Psikotes.QuestionEngine.Abstractions;
using Psikotes.QuestionEngine.Models;

namespace Psikotes.QuestionEngine.Rules;

/// <summary>
/// Executes a declarative GeneratorConfigDefinition: samples parameters,
/// evaluates operations with the safe expression evaluator, derives the
/// answer and distractors, and renders the prompt template.
/// </summary>
public sealed partial class RuleBasedGenerator : IQuestionGenerator
{
    private readonly GeneratorConfigDefinition _definition;
    private readonly string _generatorId;
    private readonly int _version;

    public RuleBasedGenerator(string generatorId, int version, GeneratorConfigDefinition definition)
    {
        _generatorId = generatorId;
        _version = version;
        _definition = definition;
    }

    public GeneratorIdentity Identity => new(_generatorId, _version, Category);

    public QuestionCategory Category { get; init; } = QuestionCategory.Numerical;

    public GeneratedQuestion Generate(Difficulty difficulty, long seed)
    {
        var random = new Random.SeededRandom(seed);
        var variables = new Dictionary<string, double>(StringComparer.Ordinal);

        foreach (var parameter in _definition.Parameters)
        {
            var max = ResolveMax(parameter, difficulty);
            var value = parameter.Integer
                ? random.Next((int)Math.Ceiling(parameter.Min), (int)Math.Floor(max) + 1)
                : parameter.Min + random.NextDouble() * (max - parameter.Min);
            variables[parameter.Name] = value;
        }

        foreach (var operation in _definition.Operations)
        {
            variables[operation.Name] = SafeExpressionEvaluator.Evaluate(operation.Expression, variables);
        }

        var answerValue = SafeExpressionEvaluator.Evaluate(_definition.AnswerExpression, variables);
        var answer = Format(answerValue);

        var options = new List<string> { answer };
        foreach (var expression in _definition.DistractorExpressions)
        {
            if (options.Count >= _definition.OptionsCount)
            {
                break;
            }

            var candidate = Format(SafeExpressionEvaluator.Evaluate(expression, variables));
            if (!options.Contains(candidate))
            {
                options.Add(candidate);
            }
        }

        var fallback = 1;
        while (options.Count < _definition.OptionsCount)
        {
            var candidate = Format(answerValue + fallback);
            if (!options.Contains(candidate))
            {
                options.Add(candidate);
            }

            fallback = fallback >= 1 ? -fallback : -fallback + 1;
        }

        var prompt = Placeholder().Replace(_definition.PromptTemplate, match =>
        {
            var value = variables[match.Groups[1].Value];
            return Format(value);
        });

        return new GeneratedQuestion(
            Identity,
            difficulty,
            seed,
            variables.ToDictionary(p => p.Key, p => Format(p.Value)),
            prompt,
            answer,
            random.Shuffle(options),
            ValidationStatus.Valid);
    }

    private static double ResolveMax(RuleParameterDefinition parameter, Difficulty difficulty)
    {
        if (parameter.MaxByDifficulty is { Length: 5 } byDifficulty)
        {
            return byDifficulty[(int)difficulty];
        }

        return parameter.Max;
    }

    private static string Format(double value)
    {
        return Math.Abs(value - Math.Round(value)) < 1e-9
            ? Math.Round(value).ToString(CultureInfo.InvariantCulture)
            : Math.Round(value, 2).ToString(CultureInfo.InvariantCulture);
    }

    [GeneratedRegex("\\{([a-zA-Z_][a-zA-Z0-9_]*)\\}")]
    private static partial Regex Placeholder();
}
