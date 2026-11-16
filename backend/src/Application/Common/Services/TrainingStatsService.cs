using Microsoft.EntityFrameworkCore;
using Psikotes.Application.Common.Interfaces;
using Psikotes.Application.Common.Models;

namespace Psikotes.Application.Common.Services;

public sealed class TrainingStatsService(IAppDbContext dbContext) : ITrainingStatsService
{
    public async Task<IReadOnlyList<CategoryStat>> GetCategoryStatsAsync(string userId, CancellationToken cancellationToken)
    {
        var events = await LoadAnsweredEventsAsync(userId, cancellationToken);

        return events
            .GroupBy(e => e.Category, StringComparer.OrdinalIgnoreCase)
            .Select(g =>
            {
                var ordered = g.OrderBy(e => e.AnsweredAtUtc).ToList();
                var (consecutiveCorrect, consecutiveIncorrect) = CountTrailingRuns(ordered);
                var correct = ordered.Count(e => e.IsCorrect);
                var answered = ordered.Count;

                return new CategoryStat(
                    g.Key,
                    answered,
                    correct,
                    answered == 0 ? 0 : Math.Round(correct * 100.0 / answered, 1),
                    consecutiveCorrect,
                    consecutiveIncorrect);
            })
            .OrderBy(s => s.Category)
            .ToList();
    }

    public async Task<IReadOnlyList<GeneratorStat>> GetGeneratorStatsAsync(string userId, CancellationToken cancellationToken)
    {
        var events = await LoadAnsweredEventsAsync(userId, cancellationToken);

        return events
            .GroupBy(e => e.GeneratorId, StringComparer.OrdinalIgnoreCase)
            .Select(g =>
            {
                var correct = g.Count(e => e.IsCorrect);
                var answered = g.Count();
                return new GeneratorStat(
                    g.Key,
                    answered,
                    correct,
                    answered == 0 ? 0 : Math.Round(correct * 100.0 / answered, 1));
            })
            .OrderBy(s => s.GeneratorId)
            .ToList();
    }

    private async Task<List<AnsweredQuestionEvent>> LoadAnsweredEventsAsync(string userId, CancellationToken cancellationToken)
    {
        return await dbContext.TrainingSessions
            .AsNoTracking()
            .Where(s => s.UserId == userId)
            .SelectMany(s => s.Questions)
            .Where(q => q.IsCorrect != null && q.AnsweredAtUtc != null)
            .OrderBy(q => q.AnsweredAtUtc)
            .Select(q => new AnsweredQuestionEvent(
                q.Category,
                q.GeneratorId,
                q.Difficulty,
                q.IsCorrect!.Value,
                q.AnsweredAtUtc!.Value))
            .ToListAsync(cancellationToken);
    }

    private static (int ConsecutiveCorrect, int ConsecutiveIncorrect) CountTrailingRuns(List<AnsweredQuestionEvent> ordered)
    {
        if (ordered.Count == 0)
        {
            return (0, 0);
        }

        var lastResult = ordered[^1].IsCorrect;
        var run = 0;
        for (var i = ordered.Count - 1; i >= 0; i--)
        {
            if (ordered[i].IsCorrect != lastResult)
            {
                break;
            }

            run++;
        }

        return lastResult ? (run, 0) : (0, run);
    }
}
