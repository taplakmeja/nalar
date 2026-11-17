using Ardalis.Result;
using Mediator;
using Psikotes.Application.UseCases.Generators.Dtos;

namespace Psikotes.Application.UseCases.Generators.Commands.Publish;

public sealed record PublishGeneratorConfigCommand(Guid Id) : ICommand<Result<GeneratorConfigDto>>;
