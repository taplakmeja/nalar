using FluentValidation;

namespace Psikotes.Application.UseCases.Generators.Commands.Rollback;

public sealed class RollbackGeneratorConfigCommandValidator : AbstractValidator<RollbackGeneratorConfigCommand>
{
    public RollbackGeneratorConfigCommandValidator()
    {
        RuleFor(x => x.GeneratorId)
            .NotEmpty().WithMessage("Generator tidak valid.");
    }
}
