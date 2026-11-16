using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Psikotes.Application.Common.Interfaces;
using Psikotes.Application.UseCases.Training.Dtos;

namespace Psikotes.Application.UseCases.Training.Queries.ListExamProfiles;

public sealed class ListExamProfilesQueryHandler(IAppDbContext dbContext)
    : IQueryHandler<ListExamProfilesQuery, Result<IReadOnlyList<ExamProfileDto>>>
{
    public async ValueTask<Result<IReadOnlyList<ExamProfileDto>>> Handle(ListExamProfilesQuery query, CancellationToken cancellationToken)
    {
        var profiles = await dbContext.ExamProfiles
            .AsNoTracking()
            .Where(p => p.IsPublic)
            .OrderBy(p => p.Name)
            .Select(p => new ExamProfileDto(
                p.Id,
                p.Slug,
                p.Name,
                p.Description,
                p.QuestionCount,
                p.DurationMinutes,
                p.PassingScorePercent))
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<ExamProfileDto>>(profiles);
    }
}
