using FluentValidation;

namespace Psikotes.Application.UseCases.Generators.Commands.Publish;

public sealed class PublishGeneratorConfigCommandValidator : AbstractValidator<PublishGeneratorConfigCommand>
{
    public PublishGeneratorConfigCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Konfigurasi tidak valid.");
    }
}
