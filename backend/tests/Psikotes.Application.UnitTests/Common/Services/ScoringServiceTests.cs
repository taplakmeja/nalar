using Psikotes.Application.Common.Services;
using Psikotes.Domain.Enums;

namespace Psikotes.Application.UnitTests.Common.Services;

public class ScoringServiceTests
{
    private readonly ScoringService _service = new();

    [Fact]
    public void PracticeMode_WeightedScore_ComputesPercent()
    {
        var score = _service.ComputeScore(8, 2, 0, TrainingMode.Practice, 60m, out var isPassed);

        Assert.False(isPassed);
        Assert.InRange(score, 0m, 100m);
    }

    [Fact]
    public void ExamSimulation_PassingScore_ReturnsPassed()
    {
        var score = _service.ComputeScore(18, 2, 0, TrainingMode.ExamSimulation, 60m, out var isPassed);

        Assert.True(isPassed);
        Assert.Equal(90m, score);
    }

    [Fact]
    public void ExamSimulation_BelowPassingScore_NotPassed()
    {
        _service.ComputeScore(5, 15, 0, TrainingMode.ExamSimulation, 60m, out var isPassed);

        Assert.False(isPassed);
    }

    [Fact]
    public void SurvivalMode_ScoresTenPerCorrect()
    {
        var score = _service.ComputeScore(7, 3, 5, TrainingMode.Survival, 60m, out _);

        Assert.Equal(70m, score);
    }

    [Fact]
    public void AllIncorrect_ScoreIsZero()
    {
        var score = _service.ComputeScore(0, 10, 0, TrainingMode.TimedTest, 60m, out _);

        Assert.Equal(0m, score);
    }

    [Fact]
    public void EmptySession_ScoreIsZero()
    {
        var score = _service.ComputeScore(0, 0, 0, TrainingMode.Practice, 60m, out var isPassed);

        Assert.Equal(0m, score);
        Assert.False(isPassed);
    }
}
