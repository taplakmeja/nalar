using FluentValidation;

namespace Psikotes.Application.UseCases.Training.Commands.SubmitAnswer;

public sealed class SubmitAnswerCommandValidator : AbstractValidator<SubmitAnswerCommand>
{
    public SubmitAnswerCommandValidator()
    {
        RuleFor(x => x.SessionId)
            .NotEmpty().WithMessage("Sesi tidak valid.");

        RuleFor(x => x.QuestionId)
            .NotEmpty().WithMessage("Soal tidak valid.");

        RuleFor(x => x.Answer)
            .NotEmpty().WithMessage("Jawaban wajib diisi.")
            .MaximumLength(500).WithMessage("Jawaban terlalu panjang.");
    }
}
