using FluentValidation;

namespace Psikotes.Application.UseCases.Generators.Commands.CreateVersion;

public sealed class CreateGeneratorConfigVersionCommandValidator : AbstractValidator<CreateGeneratorConfigVersionCommand>
{
    public CreateGeneratorConfigVersionCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty();

        RuleFor(c => c.ConfigJson)
            .NotEmpty()
            .MaximumLength(20000);
    }
}
