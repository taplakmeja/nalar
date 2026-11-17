using Ardalis.GuardClauses;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Psikotes.Application.Common.Interfaces;
using Psikotes.Application.UseCases.Generators.Commands.Create;
using Psikotes.Application.UseCases.Generators.Dtos;
using Psikotes.Domain.Entities;
using Psikotes.Domain.Enums;

namespace Psikotes.Application.UseCases.Generators.Commands.CreateVersion;

public sealed class CreateGeneratorConfigVersionCommandHandler(
    IAppDbContext dbContext,
    ICurrentUserService currentUser,
    IRuleGeneratorFactory factory) : ICommandHandler<CreateGeneratorConfigVersionCommand, Result<GeneratorConfigDto>>
{
    public async ValueTask<Result<GeneratorConfigDto>> Handle(CreateGeneratorConfigVersionCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrWhiteSpace(command.ConfigJson);

        var userId = currentUser.UserId;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result.Unauthorized("Pengguna belum masuk.");
        }

        var source = await dbContext.GeneratorConfigs
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);
        if (source is null)
        {
            return Result.NotFound("Konfigurasi generator tidak ditemukan.");
        }

        var hasOpenDraft = await dbContext.GeneratorConfigs
            .AnyAsync(c => c.GeneratorId == source.GeneratorId && c.Status == GeneratorConfigStatus.Draft, cancellationToken);
        if (hasOpenDraft)
        {
            return Result.Invalid(new ValidationError("Masih ada draft yang belum dipublikasi untuk generator ini."));
        }

        var nextVersion = await dbContext.GeneratorConfigs
            .Where(c => c.GeneratorId == source.GeneratorId)
            .MaxAsync(c => (int?)c.Version, cancellationToken) ?? 0;

        factory.Create(source.GeneratorId, nextVersion + 1, source.Category, command.ConfigJson, out var issues);
        if (issues.Count > 0)
        {
            return Result.Invalid(new ValidationError(string.Join(" ", issues)));
        }

        var config = new GeneratorConfig
        {
            Id = Guid.NewGuid(),
            GeneratorId = source.GeneratorId,
            Version = nextVersion + 1,
            Name = source.Name,
            Category = source.Category,
            ConfigJson = command.ConfigJson,
            Status = GeneratorConfigStatus.Draft,
            CreatedBy = userId,
            CreatedAtUtc = DateTime.UtcNow
        };

        dbContext.GeneratorConfigs.Add(config);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(CreateGeneratorConfigCommandHandler.ToDto(config));
    }
}
