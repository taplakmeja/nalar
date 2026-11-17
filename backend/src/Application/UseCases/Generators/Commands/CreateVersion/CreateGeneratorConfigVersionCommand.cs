using Ardalis.Result;
using Mediator;
using Psikotes.Application.UseCases.Generators.Dtos;

namespace Psikotes.Application.UseCases.Generators.Commands.CreateVersion;

public sealed record CreateGeneratorConfigVersionCommand(Guid Id, string ConfigJson)
    : ICommand<Result<GeneratorConfigDto>>;
