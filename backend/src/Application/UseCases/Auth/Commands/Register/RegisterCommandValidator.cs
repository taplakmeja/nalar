using FluentValidation;

namespace Psikotes.Application.UseCases.Auth.Commands.Register;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email wajib diisi.")
            .EmailAddress().WithMessage("Format email tidak valid.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Kata sandi wajib diisi.")
            .MinimumLength(8).WithMessage("Kata sandi minimal 8 karakter.")
            .Matches("[A-Z]").WithMessage("Kata sandi harus mengandung huruf besar.")
            .Matches("[a-z]").WithMessage("Kata sandi harus mengandung huruf kecil.")
            .Matches("[0-9]").WithMessage("Kata sandi harus mengandung angka.");

        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("Nama tampilan wajib diisi.")
            .MaximumLength(100).WithMessage("Nama tampilan maksimal 100 karakter.");
    }
}
