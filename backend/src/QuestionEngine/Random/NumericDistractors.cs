namespace Psikotes.QuestionEngine.Random;

/// <summary>
/// Generates plausible, unique numeric distractors around a correct answer.
/// Deterministic given the same SeededRandom state.
/// </summary>
public static class NumericDistractors
{
    public static IReadOnlyList<int> Create(int correct, int count, SeededRandom random, int maxOffset = 20)
    {
        var distractors = new List<int>();
        var attempts = 0;

        while (distractors.Count < count && attempts < count * 50)
        {
            attempts++;

            var magnitude = Math.Max(1, Math.Abs(correct) / 4);
            var offset = random.Next(1, Math.Max(2, Math.Min(maxOffset, magnitude + 2)));
            if (random.Chance(0.5))
            {
                offset = -offset;
            }

            var candidate = correct + offset;
            if (candidate != correct && candidate > 0 && !distractors.Contains(candidate))
            {
                distractors.Add(candidate);
            }
        }

        var fallback = 1;
        while (distractors.Count < count)
        {
            var candidate = correct + fallback++;
            if (candidate != correct && candidate > 0 && !distractors.Contains(candidate))
            {
                distractors.Add(candidate);
            }
        }

        return distractors;
    }

    public static IReadOnlyList<string> CreateStrings(int correct, int count, SeededRandom random, int maxOffset = 20) =>
        Create(correct, count, random, maxOffset)
            .Select(d => d.ToString())
            .ToList();
}
