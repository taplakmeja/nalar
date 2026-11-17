using FluentValidation;

namespace Psikotes.Application.UseCases.Admin.Settings;

public sealed class UpdateSettingCommandValidator : AbstractValidator<UpdateSettingCommand>
{
    public UpdateSettingCommandValidator()
    {
        RuleFor(c => c.Key)
            .NotEmpty()
            .Matches("^[A-Za-z0-9_.-]{1,100}$")
            .WithMessage("Kunci pengaturan hanya boleh memuat huruf, angka, titik, garis bawah, dan strip.");

        RuleFor(c => c.Value)
            .NotNull()
            .MaximumLength(2000);
    }
}
