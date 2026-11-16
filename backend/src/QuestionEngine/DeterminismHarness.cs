using Psikotes.QuestionEngine.Abstractions;
using Psikotes.QuestionEngine.Models;

namespace Psikotes.QuestionEngine;

/// <summary>
/// Verifies that a generator is deterministic: identical inputs must produce
/// an identical question. Used by automated QA before publishing a generator version.
/// </summary>
public static class DeterminismHarness
{
    public static bool Verify(IQuestionGenerator generator, Difficulty difficulty, long seed)
    {
        var first = generator.Generate(difficulty, seed);
        var second = generator.Generate(difficulty, seed);

        return first.Prompt == second.Prompt
            && first.CorrectAnswer == second.CorrectAnswer
            && first.Options.SequenceEqual(second.Options)
            && first.Parameters.SequenceEqual(second.Parameters);
    }

    public static bool VerifyAll(IQuestionGenerator generator, int samplesPerDifficulty = 25)
    {
        foreach (var difficulty in Enum.GetValues<Difficulty>())
        {
            for (var seed = 1L; seed <= samplesPerDifficulty; seed++)
            {
                if (!Verify(generator, difficulty, seed * 7919))
                {
                    return false;
                }
            }
        }

        return true;
    }
}
