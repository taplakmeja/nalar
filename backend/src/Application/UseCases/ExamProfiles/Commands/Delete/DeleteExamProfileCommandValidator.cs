using FluentValidation;

namespace Psikotes.Application.UseCases.ExamProfiles.Commands.Delete;

public sealed class DeleteExamProfileCommandValidator : AbstractValidator<DeleteExamProfileCommand>
{
    public DeleteExamProfileCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty();
    }
}
