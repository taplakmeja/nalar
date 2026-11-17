using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Psikotes.Application.Common.Interfaces;
using Psikotes.Application.UseCases.Generators.Commands.Create;
using Psikotes.Application.UseCases.Generators.Dtos;

namespace Psikotes.Application.UseCases.Generators.Queries.Get;

public sealed class GetGeneratorConfigQueryHandler(IAppDbContext dbContext)
    : IQueryHandler<GetGeneratorConfigQuery, Result<GeneratorConfigDto>>
{
    public async ValueTask<Result<GeneratorConfigDto>> Handle(GetGeneratorConfigQuery query, CancellationToken cancellationToken)
    {
        var config = await dbContext.GeneratorConfigs
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.Id == query.Id, cancellationToken);
        if (config is null)
        {
            return Result.NotFound("Konfigurasi generator tidak ditemukan.");
        }

        return Result.Success(CreateGeneratorConfigCommandHandler.ToDto(config));
    }
}
