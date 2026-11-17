using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Psikotes.Application.Common.Interfaces;
using Psikotes.Domain.Enums;

namespace Psikotes.Application.UseCases.Generators.Commands.Delete;

public sealed class DeleteGeneratorConfigCommandHandler(IAppDbContext dbContext)
    : ICommandHandler<DeleteGeneratorConfigCommand, Result>
{
    public async ValueTask<Result> Handle(DeleteGeneratorConfigCommand command, CancellationToken cancellationToken)
    {
        var config = await dbContext.GeneratorConfigs
            .SingleOrDefaultAsync(c => c.Id == command.Id, cancellationToken);
        if (config is null)
        {
            return Result.NotFound("Konfigurasi generator tidak ditemukan.");
        }

        if (config.Status is GeneratorConfigStatus.Published)
        {
            return Result.Invalid(new ValidationError("Generator yang masih terpublikasi tidak dapat dihapus. Lakukan rollback terlebih dahulu."));
        }

        dbContext.GeneratorConfigs.Remove(config);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
