using Ardalis.GuardClauses;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Psikotes.Application.Common.Interfaces;
using Psikotes.Domain.Entities;
using Psikotes.Domain.Enums;
using Psikotes.Application.UseCases.Training.Dtos;

namespace Psikotes.Application.UseCases.Training.Commands.SubmitAnswer;

public sealed class SubmitAnswerCommandHandler(
    IAppDbContext dbContext,
    ICurrentUserService currentUser) : ICommandHandler<SubmitAnswerCommand, Result<AnswerFeedback>>
{
    public async ValueTask<Result<AnswerFeedback>> Handle(SubmitAnswerCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrWhiteSpace(command.Answer);

        var session = await LoadSessionAsync(command.SessionId, cancellationToken);
        if (session is null)
        {
            return Result.NotFound("Sesi tidak ditemukan.");
        }

        if (session.Status != SessionStatus.Active)
        {
            return Result.Invalid(new ValidationError("Sesi sudah selesai."));
        }

        if (session.DeadlineUtc is not null && DateTime.UtcNow > session.DeadlineUtc.Value)
        {
            session.Status = SessionStatus.Expired;
            await dbContext.SaveChangesAsync(cancellationToken);
            return Result.Invalid(new ValidationError("Waktu sesi sudah habis."));
        }

        var question = session.Questions.SingleOrDefault(q => q.Id == command.QuestionId);
        if (question is null)
        {
            return Result.NotFound("Soal tidak ditemukan pada sesi ini.");
        }

        if (question.SubmittedAnswer is not null)
        {
            return Result.Invalid(new ValidationError("Soal ini sudah dijawab."));
        }

        var isCorrect = string.Equals(
            question.CorrectAnswer.Trim(),
            command.Answer.Trim(),
            StringComparison.OrdinalIgnoreCase);

        question.SubmittedAnswer = command.Answer.Trim();
        question.IsCorrect = isCorrect;
        question.AnsweredAtUtc = DateTime.UtcNow;

        if (isCorrect)
        {
            session.CorrectCount++;
        }
        else
        {
            session.IncorrectCount++;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new AnswerFeedback(isCorrect, question.CorrectAnswer));
    }

    private async Task<TrainingSession?> LoadSessionAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }

        return await dbContext.TrainingSessions
            .Include(s => s.Questions)
            .SingleOrDefaultAsync(s => s.Id == sessionId && s.UserId == userId, cancellationToken);
    }
}
