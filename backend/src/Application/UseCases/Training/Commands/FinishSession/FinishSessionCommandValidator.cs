using FluentValidation;

namespace Psikotes.Application.UseCases.Training.Commands.FinishSession;

public sealed class FinishSessionCommandValidator : AbstractValidator<FinishSessionCommand>
{
    public FinishSessionCommandValidator()
    {
        RuleFor(x => x.SessionId)
            .NotEmpty().WithMessage("Sesi tidak valid.");
    }
}
