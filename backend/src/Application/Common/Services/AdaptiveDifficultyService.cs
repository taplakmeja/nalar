using Psikotes.Application.Common.Interfaces;

namespace Psikotes.Application.Common.Services;

public sealed class AdaptiveDifficultyService : IAdaptiveDifficultyService
{
    private const int MinDifficulty = 0;
    private const int MaxDifficulty = 4;

    public int RecommendNextDifficulty(int currentDifficulty, CategoryStat stat)
    {
        var next = currentDifficulty;

        if (stat.ConsecutiveIncorrect >= 3)
        {
            next--;
        }
        else if (stat.ConsecutiveCorrect >= 4)
        {
            next++;
        }
        else if (stat.ConsecutiveCorrect >= 3 && stat.AccuracyPercent >= 85)
        {
            next++;
        }
        else if (stat.Answered >= 5 && stat.AccuracyPercent < 50)
        {
            next--;
        }

        return Math.Clamp(next, MinDifficulty, MaxDifficulty);
    }
}
