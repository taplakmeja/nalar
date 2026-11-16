using FluentValidation;
using Psikotes.Domain.Enums;

namespace Psikotes.Application.UseCases.Training.Commands.StartSession;

public sealed class StartSessionCommandValidator : AbstractValidator<StartSessionCommand>
{
    public StartSessionCommandValidator()
    {
        RuleFor(x => x.Mode)
            .NotEmpty().WithMessage("Mode latihan wajib dipilih.")
            .Must(BeAValidMode).WithMessage("Mode latihan tidak valid.");

        RuleFor(x => x.CategoryScope)
            .NotEmpty().WithMessage("Kategori wajib dipilih.")
            .Must(BeAValidCategory).WithMessage("Kategori tidak valid.");

        RuleFor(x => x.Difficulty)
            .InclusiveBetween(0, 4).WithMessage("Tingkat kesulitan harus antara 1 dan 5.");

        RuleFor(x => x.QuestionCount)
            .InclusiveBetween(1, 100).WithMessage("Jumlah soal harus antara 1 dan 100.");
    }

    private static bool BeAValidMode(string mode) => Enum.TryParse<TrainingMode>(mode, ignoreCase: true, out _);

    private static bool BeAValidCategory(string category) => Enum.TryParse<QuestionCategoryScope>(category, ignoreCase: true, out _);
}
