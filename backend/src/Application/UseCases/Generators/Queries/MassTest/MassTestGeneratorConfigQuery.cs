using Ardalis.Result;
using Mediator;
using Psikotes.Application.UseCases.Generators.Dtos;

namespace Psikotes.Application.UseCases.Generators.Queries.MassTest;

public sealed record MassTestGeneratorConfigQuery(Guid Id) : IQuery<Result<MassTestReportDto>>;
