using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Psikotes.Application.Common.Interfaces;
using Psikotes.Application.Common.Services;
using Psikotes.Application.UseCases.Training.Commands.FinishSession;
using Psikotes.Application.UseCases.Training.Dtos;

namespace Psikotes.Application.UseCases.Training.Queries.GetSessionResult;

public sealed class GetSessionResultQueryHandler(
    IAppDbContext dbContext,
    ICurrentUserService currentUser,
    IScoringService scoringService) : IQueryHandler<GetSessionResultQuery, Result<SessionResultDto>>
{
    private const decimal DefaultPassingScorePercent = 60m;

    public async ValueTask<Result<SessionResultDto>> Handle(GetSessionResultQuery query, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result.Unauthorized("Pengguna belum masuk.");
        }

        var session = await dbContext.TrainingSessions
            .AsNoTracking()
            .Include(s => s.Questions)
            .Include(s => s.ExamProfile)
            .SingleOrDefaultAsync(s => s.Id == query.SessionId && s.UserId == userId, cancellationToken);
        if (session is null)
        {
            return Result.NotFound("Sesi tidak ditemukan.");
        }

        return Result.Success(SessionResultMapper.ToDto(session, scoringService, DefaultPassingScorePercent));
    }
}
