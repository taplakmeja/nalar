using FluentValidation;

namespace Psikotes.Application.UseCases.Generators.Commands.Create;

public sealed class CreateGeneratorConfigCommandValidator : AbstractValidator<CreateGeneratorConfigCommand>
{
    public CreateGeneratorConfigCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nama generator wajib diisi.")
            .MaximumLength(100);

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Kategori wajib dipilih.");

        RuleFor(x => x.ConfigJson)
            .NotEmpty().WithMessage("Konfigurasi generator wajib diisi.")
            .MaximumLength(20000);
    }
}
