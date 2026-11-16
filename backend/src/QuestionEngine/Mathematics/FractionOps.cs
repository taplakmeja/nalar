namespace Psikotes.QuestionEngine.Mathematics;

public readonly record struct Fraction(int Numerator, int Denominator)
{
    public static Fraction Simplify(int numerator, int denominator)
    {
        if (denominator == 0)
        {
            throw new ArgumentException("Denominator cannot be zero.", nameof(denominator));
        }

        if (denominator < 0)
        {
            numerator = -numerator;
            denominator = -denominator;
        }

        var gcd = Gcd(System.Math.Abs(numerator), denominator);
        return new Fraction(numerator / gcd, denominator / gcd);
    }

    public static Fraction Add(Fraction left, Fraction right)
    {
        var lcm = left.Denominator / Gcd(left.Denominator, right.Denominator) * right.Denominator;
        var numerator = left.Numerator * (lcm / left.Denominator) + right.Numerator * (lcm / right.Denominator);
        return Simplify(numerator, lcm);
    }

    public override string ToString() => Denominator == 1 ? Numerator.ToString() : $"{Numerator}/{Denominator}";

    private static int Gcd(int a, int b)
    {
        while (b != 0)
        {
            (a, b) = (b, a % b);
        }
        return a;
    }
}
