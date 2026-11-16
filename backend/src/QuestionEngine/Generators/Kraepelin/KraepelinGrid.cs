namespace Psikotes.QuestionEngine.Generators.Kraepelin;

public sealed record KraepelinGrid(
    int Rows,
    int Columns,
    IReadOnlyList<int> Cells,
    int DurationMinutes)
{
    public int CellAt(int row, int column) => Cells[row * Columns + column];
}
