using Ardalis.Result;
using Mediator;
using Psikotes.Application.UseCases.Generators.Dtos;

namespace Psikotes.Application.UseCases.Generators.Queries.List;

public sealed record ListGeneratorConfigsQuery : IQuery<Result<IReadOnlyList<GeneratorConfigSummaryDto>>>;
