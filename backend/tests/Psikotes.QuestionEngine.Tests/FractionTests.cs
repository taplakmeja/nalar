using Psikotes.QuestionEngine.Mathematics;
using Xunit;

namespace Psikotes.QuestionEngine.Tests;

public class FractionTests
{
    [Fact]
    public void Add_SimplifiesResult()
    {
        var sum = Fraction.Add(new Fraction(1, 6), new Fraction(1, 3));

        Assert.Equal(1, sum.Numerator);
        Assert.Equal(2, sum.Denominator);
        Assert.Equal("1/2", sum.ToString());
    }

    [Fact]
    public void Simplify_HandlesNegativeDenominator()
    {
        var fraction = Fraction.Simplify(1, -2);

        Assert.Equal(-1, fraction.Numerator);
        Assert.Equal(2, fraction.Denominator);
    }

    [Fact]
    public void WholeNumber_PrintsWithoutDenominator()
    {
        Assert.Equal("2", Fraction.Simplify(4, 2).ToString());
    }
}
