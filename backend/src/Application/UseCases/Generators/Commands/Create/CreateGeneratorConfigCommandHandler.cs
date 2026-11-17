using Ardalis.GuardClauses;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Psikotes.Application.Common.Interfaces;
using Psikotes.Domain.Entities;
using Psikotes.Domain.Enums;
using Psikotes.Application.UseCases.Generators.Dtos;

namespace Psikotes.Application.UseCases.Generators.Commands.Create;

public sealed class CreateGeneratorConfigCommandHandler(
    IAppDbContext dbContext,
    ICurrentUserService currentUser,
    IRuleGeneratorFactory factory) : ICommandHandler<CreateGeneratorConfigCommand, Result<GeneratorConfigDto>>
{
    public async ValueTask<Result<GeneratorConfigDto>> Handle(CreateGeneratorConfigCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrWhiteSpace(command.Name);
        Guard.Against.NullOrWhiteSpace(command.Category);
        Guard.Against.NullOrWhiteSpace(command.ConfigJson);

        var userId = currentUser.UserId;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result.Unauthorized("Pengguna belum masuk.");
        }

        var generatorId = Slugify(command.Name);
        var exists = await dbContext.GeneratorConfigs
            .AnyAsync(c => c.GeneratorId == generatorId, cancellationToken);
        if (exists)
        {
            return Result.Invalid(new ValidationError($"Generator dengan nama '{command.Name}' sudah ada."));
        }

        factory.Create(generatorId, 1, command.Category, command.ConfigJson, out var issues);
        if (issues.Count > 0)
        {
            return Result.Invalid(new ValidationError(string.Join(" ", issues)));
        }

        var config = new GeneratorConfig
        {
            Id = Guid.NewGuid(),
            GeneratorId = generatorId,
            Version = 1,
            Name = command.Name.Trim(),
            Category = command.Category,
            ConfigJson = command.ConfigJson,
            Status = GeneratorConfigStatus.Draft,
            CreatedBy = userId,
            CreatedAtUtc = DateTime.UtcNow
        };

        dbContext.GeneratorConfigs.Add(config);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(ToDto(config));
    }

    private static string Slugify(string name)
    {
        var slug = new string(name.Trim().ToLowerInvariant()
            .Select(c => char.IsLetterOrDigit(c) ? c : '-').ToArray());
        while (slug.Contains("--"))
        {
            slug = slug.Replace("--", "-");
        }

        return $"custom.{slug.Trim('-')}";
    }

    internal static GeneratorConfigDto ToDto(GeneratorConfig config) =>
        new(config.Id,
            config.GeneratorId,
            config.Version,
            config.Name,
            config.Category,
            config.Status.ToString(),
            config.ConfigJson,
            config.CreatedAtUtc,
            config.PublishedAtUtc,
            config.MassTestJson,
            config.ReviewNotes);
}
