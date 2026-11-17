using FluentValidation;

namespace Psikotes.Application.UseCases.Generators.Commands.Update;

public sealed class UpdateGeneratorConfigCommandValidator : AbstractValidator<UpdateGeneratorConfigCommand>
{
    public UpdateGeneratorConfigCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Konfigurasi tidak valid.");

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
