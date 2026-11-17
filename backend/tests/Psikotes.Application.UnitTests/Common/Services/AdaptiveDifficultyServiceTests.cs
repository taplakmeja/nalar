using Psikotes.Application.Common.Interfaces;
using Psikotes.Application.Common.Services;

namespace Psikotes.Application.UnitTests.Common.Services;

public class AdaptiveDifficultyServiceTests
{
    private readonly AdaptiveDifficultyService _service = new();

    private static CategoryStat Stat(
        int answered = 10,
        double accuracy = 70,
        int consecutiveCorrect = 0,
        int consecutiveIncorrect = 0) =>
        new("Numerical", answered, 0, accuracy, consecutiveCorrect, consecutiveIncorrect);

    [Fact]
    public void FourConsecutiveCorrect_IncreasesDifficulty()
    {
        var next = _service.RecommendNextDifficulty(2, Stat(consecutiveCorrect: 4));

        Assert.Equal(3, next);
    }

    [Fact]
    public void ThreeConsecutiveCorrect_WithHighAccuracy_IncreasesDifficulty()
    {
        var next = _service.RecommendNextDifficulty(2, Stat(accuracy: 90, consecutiveCorrect: 3));

        Assert.Equal(3, next);
    }

    [Fact]
    public void ThreeConsecutiveCorrect_WithLowAccuracy_KeepsDifficulty()
    {
        var next = _service.RecommendNextDifficulty(2, Stat(accuracy: 60, consecutiveCorrect: 3));

        Assert.Equal(2, next);
    }

    [Fact]
    public void ThreeConsecutiveIncorrect_DecreasesDifficulty()
    {
        var next = _service.RecommendNextDifficulty(2, Stat(consecutiveIncorrect: 3));

        Assert.Equal(1, next);
    }

    [Fact]
    public void LowAccuracy_WithEnoughAttempts_DecreasesDifficulty()
    {
        var next = _service.RecommendNextDifficulty(2, Stat(answered: 8, accuracy: 40));

        Assert.Equal(1, next);
    }

    [Fact]
    public void AtMaxDifficulty_DoesNotExceedFour()
    {
        var next = _service.RecommendNextDifficulty(4, Stat(consecutiveCorrect: 5));

        Assert.Equal(4, next);
    }

    [Fact]
    public void AtMinDifficulty_DoesNotGoBelowZero()
    {
        var next = _service.RecommendNextDifficulty(0, Stat(consecutiveIncorrect: 5));

        Assert.Equal(0, next);
    }

    [Fact]
    public void NeutralPerformance_KeepsDifficulty()
    {
        var next = _service.RecommendNextDifficulty(2, Stat(accuracy: 70, consecutiveCorrect: 1));

        Assert.Equal(2, next);
    }
}
