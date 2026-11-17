using Ardalis.GuardClauses;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Psikotes.Application.Common.Interfaces;
using Psikotes.Domain.Enums;
using Psikotes.Application.UseCases.Generators.Commands.Create;
using Psikotes.Application.UseCases.Generators.Dtos;

namespace Psikotes.Application.UseCases.Generators.Commands.Update;

public sealed class UpdateGeneratorConfigCommandHandler(
    IAppDbContext dbContext,
    IRuleGeneratorFactory factory) : ICommandHandler<UpdateGeneratorConfigCommand, Result<GeneratorConfigDto>>
{
    public async ValueTask<Result<GeneratorConfigDto>> Handle(UpdateGeneratorConfigCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrWhiteSpace(command.Name);
        Guard.Against.NullOrWhiteSpace(command.ConfigJson);

        var config = await dbContext.GeneratorConfigs
            .SingleOrDefaultAsync(c => c.Id == command.Id, cancellationToken);
        if (config is null)
        {
            return Result.NotFound("Konfigurasi generator tidak ditemukan.");
        }

        if (config.Status != GeneratorConfigStatus.Draft)
        {
            return Result.Invalid(new ValidationError("Hanya konfigurasi berstatus Draft yang dapat diubah."));
        }

        factory.Create(config.GeneratorId, config.Version, command.Category, command.ConfigJson, out var issues);
        if (issues.Count > 0)
        {
            return Result.Invalid(new ValidationError(string.Join(" ", issues)));
        }

        config.Name = command.Name.Trim();
        config.Category = command.Category;
        config.ConfigJson = command.ConfigJson;
        config.MassTestJson = null;
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(CreateGeneratorConfigCommandHandler.ToDto(config));
    }
}
