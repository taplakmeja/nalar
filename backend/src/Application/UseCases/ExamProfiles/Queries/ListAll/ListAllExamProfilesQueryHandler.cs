using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Psikotes.Application.Common.Interfaces;
using Psikotes.Application.UseCases.ExamProfiles.Dtos;

namespace Psikotes.Application.UseCases.ExamProfiles.Queries.ListAll;

public sealed class ListAllExamProfilesQueryHandler(IAppDbContext dbContext)
    : IQueryHandler<ListAllExamProfilesQuery, Result<IReadOnlyList<AdminExamProfileDto>>>
{
    public async ValueTask<Result<IReadOnlyList<AdminExamProfileDto>>> Handle(ListAllExamProfilesQuery query, CancellationToken cancellationToken)
    {
        var profiles = await dbContext.ExamProfiles
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .Select(p => new AdminExamProfileDto(
                p.Id,
                p.Slug,
                p.Name,
                p.Description,
                p.CategoryWeightsJson,
                p.QuestionCount,
                p.DurationMinutes,
                p.PassingScorePercent,
                p.IsPublic))
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<AdminExamProfileDto>>(profiles);
    }
}
