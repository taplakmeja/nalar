using FluentValidation;

namespace Psikotes.Application.UseCases.Training.Queries.GetSessionResult;

public sealed class GetSessionResultQueryValidator : AbstractValidator<GetSessionResultQuery>
{
    public GetSessionResultQueryValidator()
    {
        RuleFor(x => x.SessionId)
            .NotEmpty().WithMessage("Sesi tidak valid.");
    }
}
