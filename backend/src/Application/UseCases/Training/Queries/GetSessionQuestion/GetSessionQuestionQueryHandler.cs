using System.Text.Json;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Psikotes.Application.Common.Interfaces;
using Psikotes.Domain.Enums;
using Psikotes.QuestionEngine;
using Psikotes.Application.UseCases.Training.Dtos;

namespace Psikotes.Application.UseCases.Training.Queries.GetSessionQuestion;

public sealed class GetSessionQuestionQueryHandler(
    IAppDbContext dbContext,
    ICurrentUserService currentUser,
    GeneratorRegistry registry) : IQueryHandler<GetSessionQuestionQuery, Result<QuestionDto>>
{
    public async ValueTask<Result<QuestionDto>> Handle(GetSessionQuestionQuery query, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result.Unauthorized("Pengguna belum masuk.");
        }

        var session = await dbContext.TrainingSessions
            .AsNoTracking()
            .Include(s => s.Questions)
            .SingleOrDefaultAsync(s => s.Id == query.SessionId && s.UserId == userId, cancellationToken);
        if (session is null)
        {
            return Result.NotFound("Sesi tidak ditemukan.");
        }

        if (session.DeadlineUtc is not null && DateTime.UtcNow > session.DeadlineUtc.Value && session.Status == SessionStatus.Active)
        {
            return Result.Invalid(new ValidationError("Waktu sesi sudah habis."));
        }

        var stored = session.Questions.SingleOrDefault(q => q.Order == query.Order);
        if (stored is null)
        {
            return Result.NotFound("Soal tidak ditemukan.");
        }

        var generator = registry.Get(stored.GeneratorId, stored.GeneratorVersion)
            ?? registry.GetLatest(stored.GeneratorId);
        if (generator is null)
        {
            return Result.CriticalError($"Generator '{stored.GeneratorId}' tidak tersedia.");
        }

        var generated = generator.Generate((Psikotes.QuestionEngine.Abstractions.Difficulty)stored.Difficulty, stored.Seed);

        return Result.Success(new QuestionDto(
            stored.Id,
            stored.Order,
            stored.Category,
            stored.Difficulty,
            generated.Prompt,
            generated.Options,
            generated.Svg));
    }
}
