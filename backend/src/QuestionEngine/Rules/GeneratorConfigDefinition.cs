using System.Text.Json;

namespace Psikotes.QuestionEngine.Rules;

/// <summary>
/// Declarative generator definition produced by the Visual Generator Builder.
/// </summary>
public sealed record GeneratorConfigDefinition(
    string PromptTemplate,
    IReadOnlyList<RuleParameterDefinition> Parameters,
    IReadOnlyList<RuleOperationDefinition> Operations,
    string AnswerExpression,
    IReadOnlyList<string> DistractorExpressions,
    int OptionsCount)
{
    public static GeneratorConfigDefinition Parse(string json)
    {
        try
        {
            var definition = JsonSerializer.Deserialize<GeneratorConfigDefinition>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return definition ?? throw new RuleExpressionException("Config is empty.");
        }
        catch (JsonException ex)
        {
            throw new RuleExpressionException($"Config is not valid JSON: {ex.Message}");
        }
    }
}
