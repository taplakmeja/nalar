using FluentValidation;

namespace Psikotes.Application.UseCases.ExamProfiles.Commands.Update;

public sealed class UpdateExamProfileCommandValidator : AbstractValidator<UpdateExamProfileCommand>
{
    public UpdateExamProfileCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty();

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
