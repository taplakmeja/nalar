using Psikotes.QuestionEngine.Rules;
using Xunit;

namespace Psikotes.QuestionEngine.Tests.Rules;

public class SafeExpressionEvaluatorTests
{
    [Theory]
    [InlineData("1 + 2", 3)]
    [InlineData("7 % 3", 1)]
    [InlineData("(2 + 3) * 4", 20)]
    [InlineData("-5 + 10", 5)]
    [InlineData("10 / 4", 2.5)]
    public void Evaluate_WithValidExpression_ReturnsExpected(string expression, double expected)
    {
        var result = SafeExpressionEvaluator.Evaluate(expression, new Dictionary<string, double>());

        Assert.Equal(expected, result, 9);
    }

    [Fact]
    public void Evaluate_WithVariable_ReturnsValue()
    {
        var variables = new Dictionary<string, double> { ["a"] = 6, ["b"] = 7 };

        var result = SafeExpressionEvaluator.Evaluate("a * b", variables);

        Assert.Equal(42, result, 9);
    }

    [Theory]
    [InlineData("1 / 0")]
    [InlineData("a % 0")]
    public void Evaluate_WithDivisionByZero_Throws(string expression)
    {
        var variables = new Dictionary<string, double> { ["a"] = 5 };

        Assert.Throws<RuleExpressionException>(() => SafeExpressionEvaluator.Evaluate(expression, variables));
    }

    [Theory]
    [InlineData("System.Console.WriteLine(1)")]
    [InlineData("evil(3)")]
    [InlineData("unknown + 1")]
    [InlineData("1; 2")]
    public void Evaluate_WithUnsafeExpression_Throws(string expression)
    {
        Assert.Throws<RuleExpressionException>(() =>
            SafeExpressionEvaluator.Evaluate(expression, new Dictionary<string, double>()));
    }
}
