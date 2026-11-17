using Psikotes.QuestionEngine.Abstractions;
using Psikotes.QuestionEngine.Models;
using Psikotes.QuestionEngine.Rules;
using Xunit;

namespace Psikotes.QuestionEngine.Tests.Rules;

public class RuleBasedGeneratorTests
{
    private const string ConfigJson = """
        {
          "promptTemplate": "Berapakah hasil dari {a} + {b}?",
          "parameters": [
            { "name": "a", "min": 1, "max": 20 },
            { "name": "b", "min": 1, "max": 20, "maxByDifficulty": [5, 10, 20, 50, 100] }
          ],
          "operations": [ { "name": "total", "expression": "a + b" } ],
          "answerExpression": "total",
          "distractorExpressions": ["total + 1", "total - 1", "total + 2"],
          "optionsCount": 4
        }
        """;

    private static RuleBasedGenerator CreateGenerator() =>
        new("custom.test-addition", 1, GeneratorConfigDefinition.Parse(ConfigJson))
        {
            Category = QuestionCategory.Numerical
        };

    [Fact]
    public void Generate_WithSameSeed_IsDeterministic()
    {
        var generator = CreateGenerator();

        var first = generator.Generate(Difficulty.Intermediate, 12345);
        var second = generator.Generate(Difficulty.Intermediate, 12345);

        Assert.Equal(first.Prompt, second.Prompt);
        Assert.Equal(first.CorrectAnswer, second.CorrectAnswer);
        Assert.Equal(first.Options, second.Options);
        Assert.Equal(first.Parameters, second.Parameters);
    }

    [Fact]
    public void Generate_UsesDifficultyScaledMax()
    {
        var generator = CreateGenerator();

        var beginner = generator.Generate(Difficulty.Beginner, 42);
        var expert = generator.Generate(Difficulty.Expert, 42);

        Assert.True(double.Parse(expert.Parameters["b"]) > 5);
        Assert.True(double.Parse(beginner.Parameters["b"]) <= 20);
    }

    [Fact]
    public void Generate_ReturnsUniqueOptionsIncludingCorrectAnswer()
    {
        var generator = CreateGenerator();

        for (var seed = 0; seed < 200; seed++)
        {
            var question = generator.Generate(Difficulty.Easy, seed);

            Assert.Equal(4, question.Options.Count);
            Assert.Equal(question.Options.Count, question.Options.Distinct().Count());
            Assert.Contains(question.CorrectAnswer, question.Options);
        }
    }

    [Fact]
    public void Generate_RendersPromptWithParameters()
    {
        var generator = CreateGenerator();

        var question = generator.Generate(Difficulty.Beginner, 7);

        Assert.Contains(double.Parse(question.Parameters["a"]).ToString(), question.Prompt);
        Assert.Contains(double.Parse(question.Parameters["b"]).ToString(), question.Prompt);
        Assert.DoesNotContain("{", question.Prompt);
    }
}
