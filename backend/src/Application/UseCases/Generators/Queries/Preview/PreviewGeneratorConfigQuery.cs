using Ardalis.Result;
using Mediator;
using Psikotes.Application.UseCases.Generators.Dtos;

namespace Psikotes.Application.UseCases.Generators.Queries.Preview;

public sealed record PreviewGeneratorConfigQuery(string ConfigJson, string Category, int Samples = 3)
    : IQuery<Result<IReadOnlyList<PreviewQuestionDto>>>;
