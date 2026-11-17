using Psikotes.QuestionEngine.Rules;
using Xunit;

namespace Psikotes.QuestionEngine.Tests.Rules;

public class RuleConfigValidatorTests
{
    [Fact]
    public void Validate_WithValidConfig_ReturnsNoIssues()
    {
        var definition = GeneratorConfigDefinition.Parse("""
        {
          "promptTemplate": "Hitung {a} * {b}",
          "parameters": [ { "name": "a", "min": 1, "max": 10 }, { "name": "b", "min": 1, "max": 10 } ],
          "operations": [ { "name": "hasil", "expression": "a * b" } ],
          "answerExpression": "hasil",
          "distractorExpressions": ["hasil + 1", "hasil - 1"],
          "optionsCount": 4
        }
        """);

        var issues = RuleConfigValidator.Validate(definition);

        Assert.Empty(issues);
    }

    [Fact]
    public void Validate_WithDuplicateParameterNames_ReportsIssue()
    {
        var definition = GeneratorConfigDefinition.Parse("""
        {
          "promptTemplate": "Hitung {a}",
          "parameters": [ { "name": "a", "min": 1, "max": 10 }, { "name": "a", "min": 1, "max": 10 } ],
          "operations": [],
          "answerExpression": "a",
          "distractorExpressions": [],
          "optionsCount": 4
        }
        """);

        var issues = RuleConfigValidator.Validate(definition);

        Assert.Contains(issues, i => i.Contains("a"));
    }

    [Fact]
    public void Validate_WithInvalidExpression_ReportsIssue()
    {
        var definition = GeneratorConfigDefinition.Parse("""
        {
          "promptTemplate": "Hitung {a}",
          "parameters": [ { "name": "a", "min": 1, "max": 10 } ],
          "operations": [ { "name": "hasil", "expression": "a / 0" } ],
          "answerExpression": "hasil",
          "distractorExpressions": [],
          "optionsCount": 4
        }
        """);

        var issues = RuleConfigValidator.Validate(definition);

        Assert.NotEmpty(issues);
    }

    [Fact]
    public void Validate_WithUnknownPromptPlaceholder_ReportsIssue()
    {
        var definition = GeneratorConfigDefinition.Parse("""
        {
          "promptTemplate": "Hitung {tidakada}",
          "parameters": [ { "name": "a", "min": 1, "max": 10 } ],
          "operations": [],
          "answerExpression": "a",
          "distractorExpressions": [],
          "optionsCount": 4
        }
        """);

        var issues = RuleConfigValidator.Validate(definition);

        Assert.Contains(issues, i => i.Contains("tidakada"));
    }

    [Fact]
    public void Validate_WithWrongMaxByDifficultyLength_ReportsIssue()
    {
        var definition = GeneratorConfigDefinition.Parse("""
        {
          "promptTemplate": "Hitung {a}",
          "parameters": [ { "name": "a", "min": 1, "max": 10, "maxByDifficulty": [5, 10, 20] } ],
          "operations": [],
          "answerExpression": "a",
          "distractorExpressions": [],
          "optionsCount": 4
        }
        """);

        var issues = RuleConfigValidator.Validate(definition);

        Assert.NotEmpty(issues);
    }
}
