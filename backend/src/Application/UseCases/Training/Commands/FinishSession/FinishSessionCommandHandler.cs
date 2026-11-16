using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Psikotes.Application.Common.Interfaces;
using Psikotes.Application.Common.Services;
using Psikotes.Domain.Enums;
using Psikotes.Application.UseCases.Training.Dtos;

namespace Psikotes.Application.UseCases.Training.Commands.FinishSession;

public sealed class FinishSessionCommandHandler(
    IAppDbContext dbContext,
    ICurrentUserService currentUser,
    IScoringService scoringService) : ICommandHandler<FinishSessionCommand, Result<SessionResultDto>>
{
    private const decimal DefaultPassingScorePercent = 60m;

    public async ValueTask<Result<SessionResultDto>> Handle(FinishSessionCommand command, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result.Unauthorized("Pengguna belum masuk.");
        }

        var session = await dbContext.TrainingSessions
            .Include(s => s.Questions)
            .SingleOrDefaultAsync(s => s.Id == command.SessionId && s.UserId == userId, cancellationToken);
        if (session is null)
        {
            return Result.NotFound("Sesi tidak ditemukan.");
        }

        if (session.Status != SessionStatus.Active)
        {
            return Result.Success(SessionResultMapper.ToDto(session, scoringService, DefaultPassingScorePercent));
        }

        var expired = session.DeadlineUtc is not null && DateTime.UtcNow > session.DeadlineUtc.Value;
        session.Status = expired ? SessionStatus.Expired : SessionStatus.Completed;
        session.CompletedAtUtc = DateTime.UtcNow;
        session.UnansweredCount = session.Questions.Count(q => q.SubmittedAnswer is null);
        session.CorrectCount = session.Questions.Count(q => q.IsCorrect == true);
        session.IncorrectCount = session.Questions.Count(q => q.IsCorrect == false);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(SessionResultMapper.ToDto(session, scoringService, DefaultPassingScorePercent));
    }
}
