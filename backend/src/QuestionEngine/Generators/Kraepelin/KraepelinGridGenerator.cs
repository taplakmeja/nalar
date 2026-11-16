using Psikotes.QuestionEngine.Random;

namespace Psikotes.QuestionEngine.Generators.Kraepelin;

/// <summary>
/// Generates a deterministic Kraepelin/Pauli continuous-arithmetic grid:
/// single digits summed column-wise, row by row, under time pressure.
/// </summary>
public sealed class KraepelinGridGenerator : IKraepelinGenerator
{
    public KraepelinGrid Generate(int rows, int columns, long seed, int durationMinutes)
    {
        var random = new SeededRandom(seed);
        var cells = new List<int>(rows * columns);

        for (var row = 0; row < rows; row++)
        {
            for (var column = 0; column < columns; column++)
            {
                cells.Add(random.Next(1, 10));
            }
        }

        return new KraepelinGrid(rows, columns, cells, durationMinutes);
    }
}
