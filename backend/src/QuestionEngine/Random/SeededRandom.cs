namespace Psikotes.QuestionEngine.Random;

/// <summary>
/// Deterministic random source. Same seed always yields the same sequence,
/// independent of machine, time, or platform.
/// </summary>
public sealed class SeededRandom(long seed)
{
    private readonly System.Random _random = new(unchecked((int)(seed & 0x7FFFFFFF) ^ (int)(seed >> 32)));

    public int Next(int minValue, int maxValue) => _random.Next(minValue, maxValue);

    public int Next(int maxValue) => _random.Next(maxValue);

    public double NextDouble() => _random.NextDouble();

    public bool Chance(double probability) => _random.NextDouble() < probability;

    public T Pick<T>(IReadOnlyList<T> items) => items[_random.Next(items.Count)];

    public IReadOnlyList<T> Shuffle<T>(IReadOnlyList<T> items)
    {
        var list = items.ToList();
        for (var i = list.Count - 1; i > 0; i--)
        {
            var j = _random.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
        return list;
    }
}
