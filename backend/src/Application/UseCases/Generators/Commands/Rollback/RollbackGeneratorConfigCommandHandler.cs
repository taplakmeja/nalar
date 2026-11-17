using Ardalis.GuardClauses;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Psikotes.Application.Common.Interfaces;
using Psikotes.Domain.Enums;
using Psikotes.Application.UseCases.Generators.Dtos;
using Psikotes.QuestionEngine;

namespace Psikotes.Application.UseCases.Generators.Commands.Rollback;

public sealed class RollbackGeneratorConfigCommandHandler(
    IAppDbContext dbContext,
    IRuleGeneratorFactory factory,
    GeneratorRegistry registry) : ICommandHandler<RollbackGeneratorConfigCommand, Result<GeneratorConfigDto>>
{
    public async ValueTask<Result<GeneratorConfigDto>> Handle(RollbackGeneratorConfigCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrWhiteSpace(command.GeneratorId);

        var current = await dbContext.GeneratorConfigs
            .Where(c => c.GeneratorId == command.GeneratorId && c.Status == GeneratorConfigStatus.Published)
            .OrderByDescending(c => c.Version)
            .FirstOrDefaultAsync(cancellationToken);
        if (current is null)
        {
            return Result.NotFound("Tidak ada versi terpublikasi untuk generator ini.");
        }

        var target = await dbContext.GeneratorConfigs
            .Where(c => c.GeneratorId == command.GeneratorId
                     && c.Status == GeneratorConfigStatus.RolledBack
                     && c.Version < current.Version)
            .OrderByDescending(c => c.Version)
            .FirstOrDefaultAsync(cancellationToken);
        if (target is null)
        {
            return Result.Invalid(new ValidationError("Tidak ada versi sebelumnya untuk di-rollback."));
        }

        current.Status = GeneratorConfigStatus.RolledBack;
        target.Status = GeneratorConfigStatus.Published;
        target.PublishedAtUtc = DateTime.UtcNow;

        registry.Deactivate(current.GeneratorId);
        var generator = factory.Create(target.GeneratorId, target.Version, target.Category, target.ConfigJson, out var issues);
        if (issues.Count == 0)
        {
            registry.Activate(generator);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new GeneratorConfigDto(
            target.Id,
            target.GeneratorId,
            target.Version,
            target.Name,
            target.Category,
            target.Status.ToString(),
            target.ConfigJson,
            target.CreatedAtUtc,
            target.PublishedAtUtc,
            target.MassTestJson,
            target.ReviewNotes));
    }
}
