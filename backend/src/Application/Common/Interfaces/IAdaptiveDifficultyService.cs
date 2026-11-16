namespace Psikotes.Application.Common.Interfaces;

public interface IAdaptiveDifficultyService
{
    int RecommendNextDifficulty(int currentDifficulty, CategoryStat stat);
}
