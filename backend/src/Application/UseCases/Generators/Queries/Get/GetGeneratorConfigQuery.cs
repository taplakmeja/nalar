using Ardalis.Result;
using Mediator;
using Psikotes.Application.UseCases.Generators.Dtos;

namespace Psikotes.Application.UseCases.Generators.Queries.Get;

public sealed record GetGeneratorConfigQuery(Guid Id) : IQuery<Result<GeneratorConfigDto>>;
