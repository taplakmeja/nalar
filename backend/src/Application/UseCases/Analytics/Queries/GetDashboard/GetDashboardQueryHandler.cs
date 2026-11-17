using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Psikotes.Application.Common.Interfaces;
using Psikotes.Domain.Enums;
using Psikotes.Application.UseCases.Analytics.Dtos;

namespace Psikotes.Application.UseCases.Analytics.Queries.GetDashboard;

public sealed class GetDashboardQueryHandler(
    IAppDbContext dbContext,
    ICurrentUserService currentUser,
    ITrainingStatsService trainingStats) : IQueryHandler<GetDashboardQuery, Result<DashboardDto>>
{
    private const int RecentSessionLimit = 10;
    private const int RecommendationMinAttempts = 5;
    private const double RecommendationAccuracyThreshold = 60.0;

    public async ValueTask<Result<DashboardDto>> Handle(GetDashboardQuery query, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result.Unauthorized("Pengguna belum masuk.");
        }

        var sessions = await dbContext.TrainingSessions
            .AsNoTracking()
            .Include(s => s.Questions)
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.StartedAtUtc)
            .ToListAsync(cancellationToken);

        var finished = sessions
            .Where(s => s.Status is SessionStatus.Completed or SessionStatus.Expired)
            .ToList();

        var answered = finished.SelectMany(s => s.Questions).Where(q => q.IsCorrect != null).ToList();
        var correct = answered.Count(q => q.IsCorrect == true);

        var categoryStats = await trainingStats.GetCategoryStatsAsync(userId, cancellationToken);
        var difficulties = answered
            .GroupBy(q => q.Difficulty)
            .OrderBy(g => g.Key)
            .Select(g =>
            {
                var total = g.Count();
                var right = g.Count(q => q.IsCorrect == true);
                return new DifficultyStatDto(
                    g.Key,
                    total,
                    right,
                    total == 0 ? 0 : Math.Round(right * 100.0 / total, 1));
            })
            .ToList();

        var averageSeconds = finished.Count == 0
            ? 0
            : Math.Round(
                finished.Sum(s => ((s.CompletedAtUtc ?? s.StartedAtUtc) - s.StartedAtUtc).TotalSeconds) / finished.Count,
                1);

        var recentSessions = finished
            .Take(RecentSessionLimit)
            .Select(s => new SessionSummaryDto(
                s.Id,
                s.Mode.ToString(),
                s.Status.ToString(),
                s.TotalQuestions,
                s.CorrectCount,
                s.IncorrectCount,
                s.UnansweredCount,
                s.Score,
                s.CompletedAtUtc))
            .ToList();

        return Result.Success(new DashboardDto(
            finished.Count,
            answered.Count,
            answered.Count == 0 ? 0 : Math.Round(correct * 100.0 / answered.Count, 1),
            averageSeconds,
            categoryStats
                .Select(s => new CategoryStatDto(
                    s.Category,
                    s.Answered,
                    s.Correct,
                    s.AccuracyPercent,
                    s.ConsecutiveCorrect,
                    s.ConsecutiveIncorrect))
                .ToList(),
            difficulties,
            recentSessions,
            BuildRecommendations(categoryStats),
            BuildReadiness(answered.Count, answered.Count == 0 ? 0 : correct * 100.0 / answered.Count)));
    }

    private static List<RecommendationDto> BuildRecommendations(IReadOnlyList<CategoryStat> stats)
    {
        var recommendations = new List<RecommendationDto>();

        var weakest = stats
            .Where(s => s.Answered >= RecommendationMinAttempts)
            .OrderBy(s => s.AccuracyPercent)
            .FirstOrDefault();

        if (weakest is null)
        {
            recommendations.Add(new RecommendationDto(
                "Mixed",
                "Mulai latihan pertama Anda untuk melihat rekomendasi."));
            return recommendations;
        }

        if (weakest.AccuracyPercent < RecommendationAccuracyThreshold)
        {
            recommendations.Add(new RecommendationDto(
                weakest.Category,
                $"Akurasi {weakest.Category} Anda {weakest.AccuracyPercent:0.#}% — fokuskan latihan di kategori ini."));
        }

        var struggling = stats.FirstOrDefault(s => s.ConsecutiveIncorrect >= 3);
        if (struggling is not null)
        {
            recommendations.Add(new RecommendationDto(
                struggling.Category,
                $"Anda salah 3 kali berturut-turut di {struggling.Category}. Coba turunkan tingkat kesulitan."));
        }

        var strongest = stats
            .Where(s => s.Answered >= RecommendationMinAttempts && s.AccuracyPercent >= 85)
            .OrderByDescending(s => s.AccuracyPercent)
            .FirstOrDefault();
        if (strongest is not null)
        {
            recommendations.Add(new RecommendationDto(
                strongest.Category,
                $"Akurasi {strongest.Category} Anda {strongest.AccuracyPercent:0.#}% — naikkan tingkat kesulitan."));
        }

        return recommendations;
    }

    private static ReadinessDto BuildReadiness(int answered, double accuracy)
    {
        if (answered < 10)
        {
            return new ReadinessDto(
                0,
                "Data Belum Cukup",
                "Selesaikan minimal 10 soal untuk estimasi kesiapan. Bersifat indikatif, bukan jaminan kelulusan.");
        }

        var label = accuracy switch
        {
            >= 80 => "Siap",
            >= 60 => "Hampir Siap",
            _ => "Belum Siap"
        };

        return new ReadinessDto(
            (int)Math.Round(accuracy),
            label,
            "Estimasi bersifat indikatif berdasarkan performa latihan, bukan jaminan kelulusan.");
    }
}
