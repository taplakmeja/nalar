using System.Text.RegularExpressions;

namespace Psikotes.QuestionEngine.Rules;

/// <summary>
/// Static validation of a rule-based generator definition: structure,
/// identifier rules, expression syntax, and template placeholders.
/// </summary>
public static partial class RuleConfigValidator
{
    [GeneratedRegex("^[a-zA-Z_][a-zA-Z0-9_]*$")]
    private static partial Regex Identifier();

    public static IReadOnlyList<string> Validate(GeneratorConfigDefinition definition)
    {
        var issues = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.PromptTemplate))
        {
            issues.Add("Prompt template is empty.");
        }

        if (definition.OptionsCount is < 2 or > 6)
        {
            issues.Add("OptionsCount must be between 2 and 6.");
        }

        if (definition.DistractorExpressions.Count == 0)
        {
            issues.Add("At least one distractor expression is required.");
        }

        var names = new HashSet<string>(StringComparer.Ordinal);
        foreach (var parameter in definition.Parameters)
        {
            if (!Identifier().IsMatch(parameter.Name))
            {
                issues.Add($"Parameter name '{parameter.Name}' is not a valid identifier.");
            }

            if (!names.Add(parameter.Name))
            {
                issues.Add($"Duplicate name '{parameter.Name}'.");
            }

            if (parameter.Min > parameter.Max)
            {
                issues.Add($"Parameter '{parameter.Name}': Min is greater than Max.");
            }

            if (parameter.MaxByDifficulty is not null && parameter.MaxByDifficulty.Length != 5)
            {
                issues.Add($"Parameter '{parameter.Name}': MaxByDifficulty must have exactly 5 entries.");
            }
        }

        foreach (var operation in definition.Operations)
        {
            if (!Identifier().IsMatch(operation.Name))
            {
                issues.Add($"Operation name '{operation.Name}' is not a valid identifier.");
            }

            if (!names.Add(operation.Name))
            {
                issues.Add($"Duplicate name '{operation.Name}'.");
            }
        }

        var variables = names.ToDictionary(n => n, _ => 1d);
        foreach (var operation in definition.Operations)
        {
            try
            {
                var value = SafeExpressionEvaluator.Evaluate(operation.Expression, variables);
                variables[operation.Name] = value;
            }
            catch (RuleExpressionException ex)
            {
                issues.Add($"Operation '{operation.Name}': {ex.Message}");
            }
        }

        try
        {
            SafeExpressionEvaluator.Evaluate(definition.AnswerExpression, variables);
        }
        catch (RuleExpressionException ex)
        {
            issues.Add($"Answer expression: {ex.Message}");
        }

        foreach (var distractor in definition.DistractorExpressions)
        {
            try
            {
                SafeExpressionEvaluator.Evaluate(distractor, variables);
            }
            catch (RuleExpressionException ex)
            {
                issues.Add($"Distractor '{distractor}': {ex.Message}");
            }
        }

        foreach (var match in Placeholder().Matches(definition.PromptTemplate))
        {
            var name = ((Match)match).Groups[1].Value;
            if (!names.Contains(name))
            {
                issues.Add($"Prompt template references unknown variable '{name}'.");
            }
        }

        return issues;
    }

    [GeneratedRegex("\\{([a-zA-Z_][a-zA-Z0-9_]*)\\}")]
    private static partial Regex Placeholder();
}
