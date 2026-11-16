using Psikotes.QuestionEngine.Abstractions;
using Psikotes.QuestionEngine.Generators.Numerical;
using Xunit;

namespace Psikotes.QuestionEngine.Tests;

public class DeterminismTests
{
    private readonly ArithmeticAdditionGenerator _generator = new();

    [Fact]
    public void SameSeed_ProducesIdenticalQuestion()
    {
        var first = _generator.Generate(Difficulty.Intermediate, seed: 123456789);
        var second = _generator.Generate(Difficulty.Intermediate, seed: 123456789);

        Assert.Equal(first.Prompt, second.Prompt);
        Assert.Equal(first.CorrectAnswer, second.CorrectAnswer);
        Assert.Equal(first.Options, second.Options);
        Assert.Equal(first.Parameters, second.Parameters);
    }

    [Fact]
    public void DifferentSeeds_ProduceDifferentQuestions()
    {
        var first = _generator.Generate(Difficulty.Intermediate, seed: 1);
        var second = _generator.Generate(Difficulty.Intermediate, seed: 2);

        Assert.NotEqual(first.Prompt, second.Prompt);
    }

    [Theory]
    [InlineData(42)]
    [InlineData(7919)]
    [InlineData(-12345)]
    public void Harness_VerifiesDeterminism(long seed)
    {
        Assert.True(DeterminismHarness.Verify(_generator, Difficulty.Advanced, seed));
    }

    [Fact]
    public void Harness_VerifiesAllDifficultiesInBulk()
    {
        Assert.True(DeterminismHarness.VerifyAll(_generator, samplesPerDifficulty: 50));
    }

    [Fact]
    public void GeneratedQuestion_HasFourUniqueOptions_IncludingCorrectAnswer()
    {
        var question = _generator.Generate(Difficulty.Beginner, seed: 999);

        Assert.Equal(4, question.Options.Count);
        Assert.Equal(question.Options.Count, question.Options.Distinct().Count());
        Assert.Contains(question.CorrectAnswer, question.Options);
        Assert.Equal(ValidationStatus.Valid, question.Validation);
    }

    [Fact]
    public void Difficulty_AffectsRange()
    {
        var beginner = _generator.Generate(Difficulty.Beginner, seed: 7);
        var expert = _generator.Generate(Difficulty.Expert, seed: 7);

        var beginnerSum = int.Parse(beginner.CorrectAnswer);
        var expertSum = int.Parse(expert.CorrectAnswer);

        Assert.True(beginnerSum < expertSum);
    }
}
