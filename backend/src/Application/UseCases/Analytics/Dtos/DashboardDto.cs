namespace Psikotes.Application.UseCases.Analytics.Dtos;

public sealed record DashboardDto(
    int SessionsCompleted,
    int TotalAnswered,
    double AccuracyPercent,
    double AverageAnswerSeconds,
    IReadOnlyList<CategoryStatDto> Categories,
    IReadOnlyList<DifficultyStatDto> Difficulties,
    IReadOnlyList<SessionSummaryDto> RecentSessions,
    IReadOnlyList<RecommendationDto> Recommendations,
    ReadinessDto Readiness);
