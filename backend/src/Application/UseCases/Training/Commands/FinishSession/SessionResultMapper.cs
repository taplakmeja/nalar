using Psikotes.Application.Common.Interfaces;
using Psikotes.Application.UseCases.Training.Dtos;
using Psikotes.Domain.Entities;

namespace Psikotes.Application.UseCases.Training.Commands.FinishSession;

public static class SessionResultMapper
{
    public static SessionResultDto ToDto(TrainingSession session, IScoringService scoringService, decimal defaultPassingScorePercent)
    {
        var passingScore = session.ExamProfile?.PassingScorePercent ?? defaultPassingScorePercent;
        var score = scoringService.ComputeScore(
            session.CorrectCount,
            session.IncorrectCount,
            session.UnansweredCount,
            session.Mode,
            passingScore,
            out var isPassed);

        return new SessionResultDto(
            session.Id,
            session.Mode.ToString(),
            session.Status.ToString(),
            session.TotalQuestions,
            session.CorrectCount,
            session.IncorrectCount,
            session.UnansweredCount,
            score,
            isPassed,
            session.DeadlineUtc,
            session.Questions
                .OrderBy(q => q.Order)
                .Select(q => new SessionQuestionResultDto(
                    q.Order,
                    q.Category,
                    q.GeneratorId,
                    q.Difficulty,
                    q.IsCorrect ?? false,
                    q.SubmittedAnswer,
                    q.CorrectAnswer))
                .ToList());
    }
}
