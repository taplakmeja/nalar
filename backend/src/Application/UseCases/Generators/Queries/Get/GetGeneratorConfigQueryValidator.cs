using FluentValidation;

namespace Psikotes.Application.UseCases.Generators.Queries.Get;

public sealed class GetGeneratorConfigQueryValidator : AbstractValidator<GetGeneratorConfigQuery>
{
    public GetGeneratorConfigQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Konfigurasi tidak valid.");
    }
}
