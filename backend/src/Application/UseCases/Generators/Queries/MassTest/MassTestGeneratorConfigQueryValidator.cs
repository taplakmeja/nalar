using FluentValidation;

namespace Psikotes.Application.UseCases.Generators.Queries.MassTest;

public sealed class MassTestGeneratorConfigQueryValidator : AbstractValidator<MassTestGeneratorConfigQuery>
{
    public MassTestGeneratorConfigQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Konfigurasi tidak valid.");
    }
}
