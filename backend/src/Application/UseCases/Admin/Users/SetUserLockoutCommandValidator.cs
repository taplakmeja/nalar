using FluentValidation;

namespace Psikotes.Application.UseCases.Admin.Users;

public sealed class SetUserLockoutCommandValidator : AbstractValidator<SetUserLockoutCommand>
{
    public SetUserLockoutCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Pengguna tidak valid.");
    }
}
