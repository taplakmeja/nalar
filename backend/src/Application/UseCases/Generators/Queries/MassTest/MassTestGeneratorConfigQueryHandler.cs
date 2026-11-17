using System.Text.Json;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Psikotes.Application.Common.Interfaces;
using Psikotes.QuestionEngine.Qa;
using Psikotes.Application.UseCases.Generators.Dtos;

namespace Psikotes.Application.UseCases.Generators.Queries.MassTest;

public sealed class MassTestGeneratorConfigQueryHandler(
    IAppDbContext dbContext,
    IRuleGeneratorFactory factory) : IQueryHandler<MassTestGeneratorConfigQuery, Result<MassTestReportDto>>
{
    private const int MaxDuplicateRatePercent = 20;

    public async ValueTask<Result<MassTestReportDto>> Handle(MassTestGeneratorConfigQuery query, CancellationToken cancellationToken)
    {
        var config = await dbContext.GeneratorConfigs
            .SingleOrDefaultAsync(c => c.Id == query.Id, cancellationToken);
        if (config is null)
        {
            return Result.NotFound("Konfigurasi generator tidak ditemukan.");
        }

        var generator = factory.Create(config.GeneratorId, config.Version, config.Category, config.ConfigJson, out var issues);
        if (issues.Count > 0)
        {
            return Result.Invalid(new ValidationError(string.Join(" ", issues)));
        }

        var report = MassTester.Run(generator, samplesPerDifficulty: 50);
        var dto = new MassTestReportDto(
            report.GeneratorId,
            report.GeneratorVersion,
            report.TotalQuestions,
            report.DistinctQuestions,
            report.DuplicateRate,
            report.InvalidCount,
            report.Deterministic,
            report.Issues,
            report.Deterministic && report.InvalidCount == 0 && report.DuplicateRate <= MaxDuplicateRatePercent);

        config.MassTestJson = JsonSerializer.Serialize(dto);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(dto);
    }
}
