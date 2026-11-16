using FluentValidation;

namespace Psikotes.Application.UseCases.Auth.Commands.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email wajib diisi.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Kata sandi wajib diisi.");
    }
}
