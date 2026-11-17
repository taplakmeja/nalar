using FluentValidation;

namespace Psikotes.Application.UseCases.ExamProfiles.Commands.Create;

public sealed class CreateExamProfileCommandValidator : AbstractValidator<CreateExamProfileCommand>
{
    public CreateExamProfileCommandValidator()
    {
        RuleFor(c => c.Slug)
            .NotEmpty()
            .Matches("^[a-z0-9-]{3,60}$")
            .WithMessage("Slug hanya boleh memuat huruf kecil, angka, dan strip (3-60 karakter).");

        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(120);

        RuleFor(c => c.Description)
            .MaximumLength(500);

        RuleFor(c => c.CategoryWeightsJson)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(c => c.QuestionCount)
            .InclusiveBetween(1, 500);

        RuleFor(c => c.DurationMinutes)
            .InclusiveBetween(1, 600);

        RuleFor(c => c.PassingScorePercent)
            .InclusiveBetween(0, 100);
    }
}
