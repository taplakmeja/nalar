using System.Globalization;

namespace Psikotes.QuestionEngine.Rendering;

/// <summary>
/// Deterministic, minimal SVG rendering helpers for abstract/spatial questions.
/// Output depends only on the supplied arguments — never on time or environment.
/// </summary>
public static class SvgBuilder
{
    public static string Wrap(string inner, int size = 80) =>
        $"<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 {size} {size}\" width=\"{size}\" height=\"{size}\">{inner}</svg>";

    public static string Polygon(IReadOnlyList<(int X, int Y)> points, string fill, double rotationDegrees = 0)
    {
        var coordinates = string.Join(" ", points.Select(p => $"{p.X},{p.Y}"));
        var transform = rotationDegrees == 0
            ? string.Empty
            : $" transform=\"rotate({rotationDegrees.ToString(CultureInfo.InvariantCulture)} 40 40)\"";
        return $"<polygon points=\"{coordinates}\" fill=\"{fill}\" stroke=\"#1e293b\" stroke-width=\"2\"{transform}/>";
    }

    public static string Circle(string fill, int cx = 40, int cy = 40, int r = 24) =>
        $"<circle cx=\"{cx}\" cy=\"{cy}\" r=\"{r}\" fill=\"{fill}\" stroke=\"#1e293b\" stroke-width=\"2\"/>";

    public static string Square(string fill, double rotationDegrees = 0)
    {
        var transform = rotationDegrees == 0
            ? string.Empty
            : $" transform=\"rotate({rotationDegrees.ToString(CultureInfo.InvariantCulture)} 40 40)\"";
        return $"<rect x=\"18\" y=\"18\" width=\"44\" height=\"44\" fill=\"{fill}\" stroke=\"#1e293b\" stroke-width=\"2\"{transform}/>";
    }

    public static string Triangle(string fill, double rotationDegrees = 0) =>
        Polygon([(40, 12), (68, 64), (12, 64)], fill, rotationDegrees);

    public static string CubeStack(IReadOnlyList<int> columnHeights)
    {
        var width = 40 + columnHeights.Count * 30;
        var maxHeight = columnHeights.Max();
        var height = 40 + maxHeight * 30;
        var cubes = new List<string>();

        for (var column = 0; column < columnHeights.Count; column++)
        {
            for (var level = 0; level < columnHeights[column]; level++)
            {
                var x = 20 + column * 30;
                var y = height - 30 - level * 30 - 10;
                cubes.Add($"<rect x=\"{x}\" y=\"{y}\" width=\"26\" height=\"26\" fill=\"#94a3b8\" stroke=\"#1e293b\" stroke-width=\"2\"/>");
            }
        }

        return $"<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 {width} {height}\" width=\"{width}\" height=\"{height}\">{string.Join(string.Empty, cubes)}</svg>";
    }
}
