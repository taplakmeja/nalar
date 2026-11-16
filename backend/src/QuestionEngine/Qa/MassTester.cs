using Psikotes.QuestionEngine.Abstractions;
using Psikotes.QuestionEngine.Models;
using Psikotes.QuestionEngine.Validation;

namespace Psikotes.QuestionEngine.Qa;

/// <summary>
/// Layer-2 automated QA: bulk generation with duplicate detection,
/// technical validation, and determinism verification.
/// </summary>
public static class MassTester
{
    public static MassTestReport Run(IQuestionGenerator generator, int samplesPerDifficulty = 100)
    {
        var issues = new List<string>();
        var signatures = new HashSet<string>();
        var total = 0;
        var invalid = 0;

        foreach (var difficulty in Enum.GetValues<Difficulty>())
        {
            for (var i = 0; i < samplesPerDifficulty; i++)
            {
                var seed = i * 10_003L + (int)difficulty * 7919 + 17;
                var question = generator.Generate(difficulty, seed);
                total++;

                if (TechnicalValidator.Validate(question) == ValidationStatus.Invalid)
                {
                    invalid++;
                }

                signatures.Add($"{question.Prompt}|{question.CorrectAnswer}|{string.Join(",", question.Options)}");
            }
        }

        var deterministic = DeterminismHarness.VerifyAll(generator, samplesPerDifficulty: 10);
        if (!deterministic)
        {
            issues.Add("Generator is not deterministic.");
        }

        if (invalid > 0)
        {
            issues.Add($"{invalid} questions failed technical validation.");
        }

        var duplicateRate = total == 0 ? 0 : (total - signatures.Count) * 100 / total;

        return new MassTestReport(
            generator.Identity.Id,
            generator.Identity.Version,
            total,
            signatures.Count,
            duplicateRate,
            invalid,
            deterministic,
            issues);
    }
}
