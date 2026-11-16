using Psikotes.Domain.Enums;

namespace Psikotes.Application.Common.Interfaces;

public interface IScoringService
{
    decimal ComputeScore(int correct, int incorrect, int unanswered, TrainingMode mode, decimal passingScorePercent, out bool isPassed);
}
