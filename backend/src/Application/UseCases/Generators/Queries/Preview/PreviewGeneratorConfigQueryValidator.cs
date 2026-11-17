using FluentValidation;

namespace Psikotes.Application.UseCases.Generators.Queries.Preview;

public sealed class PreviewGeneratorConfigQueryValidator : AbstractValidator<PreviewGeneratorConfigQuery>
{
    public PreviewGeneratorConfigQueryValidator()
    {
        RuleFor(x => x.ConfigJson)
            .NotEmpty().WithMessage("Konfigurasi wajib diisi.")
            .MaximumLength(20000);

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Kategori wajib dipilih.");

        RuleFor(x => x.Samples)
            .InclusiveBetween(1, 20).WithMessage("Jumlah contoh harus antara 1 dan 20.");
    }
}
