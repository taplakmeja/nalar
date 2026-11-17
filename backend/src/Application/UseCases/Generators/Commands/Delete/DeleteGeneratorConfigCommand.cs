using Ardalis.Result;
using Mediator;

namespace Psikotes.Application.UseCases.Generators.Commands.Delete;

public sealed record DeleteGeneratorConfigCommand(Guid Id) : ICommand<Result>;
