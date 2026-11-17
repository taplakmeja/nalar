using System.Text.Json;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Psikotes.Application.Common.Interfaces;
using Psikotes.Domain.Enums;
using Psikotes.Application.UseCases.Generators.Dtos;
using Psikotes.QuestionEngine;

namespace Psikotes.Application.UseCases.Generators.Commands.Publish;

public sealed class PublishGeneratorConfigCommandHandler(
    IAppDbContext dbContext,
    IRuleGeneratorFactory factory,
    GeneratorRegistry registry) : ICommandHandler<PublishGeneratorConfigCommand, Result<GeneratorConfigDto>>
{
    private const int MaxDuplicateRatePercent = 20;

    public async ValueTask<Result<GeneratorConfigDto>> Handle(PublishGeneratorConfigCommand command, CancellationToken cancellationToken)
    {
        var config = await dbContext.GeneratorConfigs
            .SingleOrDefaultAsync(c => c.Id == command.Id, cancellationToken);
        if (config is null)
        {
            return Result.NotFound("Konfigurasi generator tidak ditemukan.");
        }

        if (config.Status != GeneratorConfigStatus.Draft)
        {
            return Result.Invalid(new ValidationError("Hanya konfigurasi berstatus Draft yang dapat dipublikasi."));
        }

        if (string.IsNullOrWhiteSpace(config.MassTestJson))
        {
            return Result.Invalid(new ValidationError("Jalankan mass test terlebih dahulu sebelum publikasi."));
        }

        var report = JsonSerializer.Deserialize<MassTestReportDto>(config.MassTestJson);
        if (report is null || !report.Passed)
        {
            return Result.Invalid(new ValidationError("Mass test belum lulus. Perbaiki masalah terlebih dahulu."));
        }

        var previous = await dbContext.GeneratorConfigs
            .Where(c => c.GeneratorId == config.GeneratorId
                     && c.Status == GeneratorConfigStatus.Published
                     && c.Version < config.Version)
            .ToListAsync(cancellationToken);
        foreach (var old in previous)
        {
            old.Status = GeneratorConfigStatus.RolledBack;
        }

        config.Status = GeneratorConfigStatus.Published;
        config.PublishedAtUtc = DateTime.UtcNow;

        var generator = factory.Create(config.GeneratorId, config.Version, config.Category, config.ConfigJson, out var issues);
        if (issues.Count > 0)
        {
            return Result.Invalid(new ValidationError(string.Join(" ", issues)));
        }

        registry.Activate(generator);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new GeneratorConfigDto(
            config.Id,
            config.GeneratorId,
            config.Version,
            config.Name,
            config.Category,
            config.Status.ToString(),
            config.ConfigJson,
            config.CreatedAtUtc,
            config.PublishedAtUtc,
            config.MassTestJson,
            config.ReviewNotes));
    }
}
