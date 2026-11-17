using Ardalis.Result;
using Mediator;
using Psikotes.Application.UseCases.Generators.Dtos;

namespace Psikotes.Application.UseCases.Generators.Commands.Create;

public sealed record CreateGeneratorConfigCommand(
    string Name,
    string Category,
    string ConfigJson) : ICommand<Result<GeneratorConfigDto>>;
