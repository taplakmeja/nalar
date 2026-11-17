using System.Text.Json;
using Ardalis.GuardClauses;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Psikotes.Application.Common.Interfaces;
using Psikotes.Application.Common.Models;
using Psikotes.Application.UseCases.Training.Dtos;
using Psikotes.Domain.Entities;
using Psikotes.Domain.Enums;
using Psikotes.QuestionEngine.Abstractions;
using Psikotes.QuestionEngine.Models;

namespace Psikotes.Application.UseCases.Training.Commands.StartSession;

public sealed class StartSessionCommandHandler(
    IAppDbContext dbContext,
    ICurrentUserService currentUser,
    IQuestionGenerationService questionGeneration,
    ITrainingStatsService trainingStats,
    IAdaptiveDifficultyService adaptiveDifficulty) : ICommandHandler<StartSessionCommand, Result<StartSessionResponse>>
{
    private static readonly QuestionCategory[] AllCategories =
    [
        QuestionCategory.Numerical,
        QuestionCategory.Verbal,
        QuestionCategory.Logical,
        QuestionCategory.Abstract,
        QuestionCategory.Spatial,
        QuestionCategory.Kraepelin
    ];

    public async ValueTask<Result<StartSessionResponse>> Handle(StartSessionCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrWhiteSpace(command.Mode);
        Guard.Against.NullOrWhiteSpace(command.CategoryScope);

        if (string.IsNullOrWhiteSpace(currentUser.UserId))
        {
            return Result.Unauthorized("Pengguna belum masuk.");
        }

        if (!Enum.TryParse<TrainingMode>(command.Mode, ignoreCase: true, out var mode))
        {
            return Result.Invalid(new ValidationError("Mode latihan tidak valid."));
        }

        if (!Enum.TryParse<QuestionCategoryScope>(command.CategoryScope, ignoreCase: true, out var scope))
        {
            return Result.Invalid(new ValidationError("Kategori tidak valid."));
        }

        var questionCount = command.QuestionCount;
        var durationMinutes = mode switch
        {
            TrainingMode.TimedTest => 25,
            TrainingMode.ExamSimulation => 60,
            TrainingMode.DailyChallenge => 15,
            TrainingMode.Survival => 10,
            _ => (int?)null
        };

        ExamProfile? profile = null;
        if (!string.IsNullOrWhiteSpace(command.ExamProfileSlug))
        {
            profile = await dbContext.ExamProfiles
                .SingleOrDefaultAsync(p => p.Slug == command.ExamProfileSlug, cancellationToken);
            if (profile is null)
            {
                return Result.NotFound($"Profil ujian '{command.ExamProfileSlug}' tidak ditemukan.");
            }

            questionCount = profile.QuestionCount;
            durationMinutes = profile.DurationMinutes;
        }

        var difficulty = command.Difficulty;
        if (mode == TrainingMode.WeaknessTraining || command.UseAdaptiveDifficulty)
        {
            var categoryStats = await trainingStats.GetCategoryStatsAsync(currentUser.UserId, cancellationToken);
            difficulty = ResolveAdaptiveDifficulty(scope, difficulty, categoryStats);
        }

        if (mode == TrainingMode.WeaknessTraining && scope == QuestionCategoryScope.Mixed)
        {
            scope = await ResolveWeakestCategoryAsync(currentUser.UserId, cancellationToken);
        }

        var now = DateTime.UtcNow;
        var sessionSeed = Random.Shared.NextInt64();
        var session = new TrainingSession
        {
            Id = Guid.NewGuid(),
            UserId = currentUser.UserId,
            Mode = mode,
            CategoryScope = scope,
            DifficultyScope = difficulty,
            Status = SessionStatus.Active,
            StartedAtUtc = now,
            DeadlineUtc = durationMinutes is null ? null : now.AddMinutes(durationMinutes.Value),
            TotalQuestions = questionCount,
            ExamProfileId = profile?.Id
        };

        var categories = scope == QuestionCategoryScope.Mixed
            ? AllCategories
            : [Enum.Parse<QuestionCategory>(scope.ToString())];

        for (var order = 1; order <= questionCount; order++)
        {
            var category = categories[(order - 1) % categories.Length];
            var seed = unchecked(sessionSeed + order);
            var generated = questionGeneration.Generate(category.ToString(), difficulty, seed);

            session.Questions.Add(new SessionQuestion
            {
                Id = Guid.NewGuid(),
                SessionId = session.Id,
                Order = order,
                Category = generated.Generator.Category.ToString(),
                GeneratorId = generated.Generator.Id,
                GeneratorVersion = generated.Generator.Version,
                Seed = generated.Seed,
                Difficulty = (int)generated.Difficulty,
                ParametersJson = JsonSerializer.Serialize(generated.Parameters),
                CorrectAnswer = generated.CorrectAnswer,
                OptionsJson = JsonSerializer.Serialize(generated.Options)
            });
        }

        dbContext.TrainingSessions.Add(session);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new StartSessionResponse(
            session.Id,
            mode.ToString(),
            scope.ToString(),
            questionCount,
            session.DeadlineUtc));
    }

    private int ResolveAdaptiveDifficulty(
        QuestionCategoryScope scope,
        int requestedDifficulty,
        IReadOnlyList<CategoryStat> categoryStats)
    {
        if (scope == QuestionCategoryScope.Mixed)
        {
            var answered = categoryStats.Sum(s => s.Answered);
            if (answered == 0)
            {
                return requestedDifficulty;
            }

            var correct = categoryStats.Sum(s => s.Correct);
            var accuracy = correct * 100.0 / answered;
            var consecutiveCorrect = categoryStats.Min(s => s.ConsecutiveCorrect);
            var consecutiveIncorrect = categoryStats.Max(s => s.ConsecutiveIncorrect);

            return adaptiveDifficulty.RecommendNextDifficulty(
                requestedDifficulty,
                new CategoryStat("Mixed", answered, correct, Math.Round(accuracy, 1), consecutiveCorrect, consecutiveIncorrect));
        }

        var stat = categoryStats.FirstOrDefault(
            s => s.Category.Equals(scope.ToString(), StringComparison.OrdinalIgnoreCase));

        return stat is null
            ? requestedDifficulty
            : adaptiveDifficulty.RecommendNextDifficulty(requestedDifficulty, stat);
    }

    private async Task<QuestionCategoryScope> ResolveWeakestCategoryAsync(string userId, CancellationToken cancellationToken)
    {
        var stats = await trainingStats.GetCategoryStatsAsync(userId, cancellationToken);
        if (stats.Count == 0)
        {
            return QuestionCategoryScope.Mixed;
        }

        var weakest = stats
            .Where(s => s.Answered >= 3)
            .OrderBy(s => s.AccuracyPercent)
            .FirstOrDefault();

        if (weakest is not null)
        {
            return Enum.Parse<QuestionCategoryScope>(weakest.Category, ignoreCase: true);
        }

        var leastAttempted = stats.OrderBy(s => s.Answered).First();
        return Enum.Parse<QuestionCategoryScope>(leastAttempted.Category, ignoreCase: true);
    }
}
