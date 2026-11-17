using Ardalis.Result;
using Mediator;
using Psikotes.Application.UseCases.Generators.Dtos;

namespace Psikotes.Application.UseCases.Generators.Commands.Update;

public sealed record UpdateGeneratorConfigCommand(
    Guid Id,
    string Name,
    string Category,
    string ConfigJson) : ICommand<Result<GeneratorConfigDto>>;
