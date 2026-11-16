using FluentValidation;

namespace Psikotes.Application.UseCases.Auth.Commands.Refresh;

public sealed class RefreshCommandValidator : AbstractValidator<RefreshCommand>
{
    public RefreshCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token wajib diisi.");
    }
}
