using FluentValidation;
using Psikotes.Domain.Common;

namespace Psikotes.Application.UseCases.Admin.Users;

public sealed class SetUserRolesCommandValidator : AbstractValidator<SetUserRolesCommand>
{
    public SetUserRolesCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Pengguna tidak valid.");

        RuleFor(x => x.Roles)
            .NotNull().WithMessage("Peran wajib dipilih.")
            .Must(BeKnownRoles).WithMessage("Peran tidak valid.");
    }

    private static bool BeKnownRoles(IReadOnlyList<string> roles) =>
        roles.All(role => Roles.All.Contains(role, StringComparer.OrdinalIgnoreCase));
}
