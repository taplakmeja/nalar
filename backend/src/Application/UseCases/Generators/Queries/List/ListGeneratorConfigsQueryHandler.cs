using System.Text.Json;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Psikotes.Application.Common.Interfaces;
using Psikotes.Application.UseCases.Generators.Dtos;

namespace Psikotes.Application.UseCases.Generators.Queries.List;

public sealed class ListGeneratorConfigsQueryHandler(IAppDbContext dbContext)
    : IQueryHandler<ListGeneratorConfigsQuery, Result<IReadOnlyList<GeneratorConfigSummaryDto>>>
{
    public async ValueTask<Result<IReadOnlyList<GeneratorConfigSummaryDto>>> Handle(ListGeneratorConfigsQuery query, CancellationToken cancellationToken)
    {
        var configs = await dbContext.GeneratorConfigs
            .AsNoTracking()
            .OrderBy(c => c.GeneratorId)
            .ThenByDescending(c => c.Version)
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<GeneratorConfigSummaryDto>>(configs.Select(c =>
        {
            var passed = false;
            if (!string.IsNullOrWhiteSpace(c.MassTestJson))
            {
                passed = JsonSerializer.Deserialize<Dtos.MassTestReportDto>(c.MassTestJson)?.Passed ?? false;
            }

            return new GeneratorConfigSummaryDto(
                c.Id,
                c.GeneratorId,
                c.Version,
                c.Name,
                c.Category,
                c.Status.ToString(),
                c.CreatedAtUtc,
                c.PublishedAtUtc,
                passed);
        }).ToList());
    }
}
