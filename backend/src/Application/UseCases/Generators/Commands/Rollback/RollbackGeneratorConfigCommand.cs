using Ardalis.Result;
using Mediator;
using Psikotes.Application.UseCases.Generators.Dtos;

namespace Psikotes.Application.UseCases.Generators.Commands.Rollback;

public sealed record RollbackGeneratorConfigCommand(string GeneratorId) : ICommand<Result<GeneratorConfigDto>>;
