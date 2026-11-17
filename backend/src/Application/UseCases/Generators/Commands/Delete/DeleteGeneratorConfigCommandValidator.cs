using FluentValidation;

namespace Psikotes.Application.UseCases.Generators.Commands.Delete;

public sealed class DeleteGeneratorConfigCommandValidator : AbstractValidator<DeleteGeneratorConfigCommand>
{
    public DeleteGeneratorConfigCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Konfigurasi tidak valid.");
    }
}
