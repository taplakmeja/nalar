namespace Psikotes.Application.Common.Interfaces;

public sealed record CategoryStat(
    string Category,
    int Answered,
    int Correct,
    double AccuracyPercent,
    int ConsecutiveCorrect,
    int ConsecutiveIncorrect);

public sealed record GeneratorStat(
    string GeneratorId,
    int Answered,
    int Correct,
    double AccuracyPercent);

public interface ITrainingStatsService
{
    Task<IReadOnlyList<CategoryStat>> GetCategoryStatsAsync(string userId, CancellationToken cancellationToken);

    Task<IReadOnlyList<GeneratorStat>> GetGeneratorStatsAsync(string userId, CancellationToken cancellationToken);
}
