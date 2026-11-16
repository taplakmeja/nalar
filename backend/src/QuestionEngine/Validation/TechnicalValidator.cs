using Psikotes.QuestionEngine.Abstractions;
using Psikotes.QuestionEngine.Models;

namespace Psikotes.QuestionEngine.Validation;

/// <summary>
/// Layer-1 quality assurance: structural validation of a generated question.
/// Verifies answer/option structure, option uniqueness, and metadata presence.
/// </summary>
public static class TechnicalValidator
{
    public static ValidationStatus Validate(GeneratedQuestion question)
    {
        if (string.IsNullOrWhiteSpace(question.Prompt) ||
            string.IsNullOrWhiteSpace(question.CorrectAnswer) ||
            question.Options.Count < 2 ||
            question.Options.Distinct().Count() != question.Options.Count ||
            !question.Options.Contains(question.CorrectAnswer) ||
            string.IsNullOrWhiteSpace(question.Generator.Id) ||
            question.Generator.Version < 1)
        {
            return ValidationStatus.Invalid;
        }

        return ValidationStatus.Valid;
    }

    public static GeneratedQuestion EnsureValidated(GeneratedQuestion question)
    {
        var status = Validate(question);
        if (status == ValidationStatus.Invalid)
        {
            throw new InvalidOperationException(
                $"Generated question from '{question.Generator.Id}' failed technical validation.");
        }

        return question;
    }
}
