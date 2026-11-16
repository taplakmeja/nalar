using Psikotes.Application.Common.Interfaces;
using Psikotes.Domain.Enums;

namespace Psikotes.Application.Common.Services;

public class ScoringService : IScoringService
{
    private static readonly IReadOnlyList<decimal> DifficultyWeights = [1.0m, 1.2m, 1.5m, 1.8m, 2.0m];

    public decimal ComputeScore(int correct, int incorrect, int unanswered, TrainingMode mode, decimal passingScorePercent, out bool isPassed)
    {
        var total = correct + incorrect + unanswered;
        if (total == 0)
        {
            isPassed = false;
            return 0m;
        }

        isPassed = mode switch
        {
            TrainingMode.ExamSimulation or TrainingMode.DailyChallenge => ComputePercent(correct, incorrect, unanswered) >= passingScorePercent,
            _ => false
        };

        return mode switch
        {
            TrainingMode.Survival => correct * 10m,
            TrainingMode.ExamSimulation or TrainingMode.DailyChallenge => ComputePercent(correct, incorrect, unanswered),
            _ => ComputeWeighted(correct, incorrect, unanswered)
        };
    }

    private static decimal ComputePercent(int correct, int incorrect, int unanswered)
    {
        var total = correct + incorrect + unanswered;
        return Math.Round(correct * 100m / total, 2);
    }

    private static decimal ComputeWeighted(int correct, int incorrect, int unanswered)
    {
        var total = correct + incorrect + unanswered;
        var maxWeight = DifficultyWeights[^1];
        var earned = correct * maxWeight - incorrect * (maxWeight / 4m);
        var possible = total * maxWeight;
        var raw = Math.Max(0m, earned) * 100m / possible;
        return Math.Round(raw, 2);
    }
}
