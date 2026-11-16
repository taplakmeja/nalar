using FluentValidation;

namespace Psikotes.Application.UseCases.Training.Queries.GetSessionQuestion;

public sealed class GetSessionQuestionQueryValidator : AbstractValidator<GetSessionQuestionQuery>
{
    public GetSessionQuestionQueryValidator()
    {
        RuleFor(x => x.SessionId)
            .NotEmpty().WithMessage("Sesi tidak valid.");

        RuleFor(x => x.Order)
            .InclusiveBetween(1, 1000).WithMessage("Nomor soal tidak valid.");
    }
}
