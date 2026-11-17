namespace Psikotes.QuestionEngine.Rules;

/// <summary>
/// Minimal arithmetic expression evaluator for rule-based generators.
/// Supports integer/decimal literals, variables, + - * / %, unary minus,
/// and parentheses. No function calls, member access, or indexing —
/// anything beyond plain arithmetic throws RuleExpressionException.
/// </summary>
public static class SafeExpressionEvaluator
{
    public static double Evaluate(string expression, IReadOnlyDictionary<string, double> variables)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            throw new RuleExpressionException("Expression is empty.");
        }

        var parser = new Parser(Tokenize(expression), variables);
        var value = parser.ParseExpression();
        parser.ExpectEnd();

        if (!double.IsFinite(value))
        {
            throw new RuleExpressionException($"Expression '{expression}' produced a non-finite value.");
        }

        return value;
    }

    public static bool TryEvaluate(string expression, IReadOnlyDictionary<string, double> variables, out double value)
    {
        try
        {
            value = Evaluate(expression, variables);
            return true;
        }
        catch (RuleExpressionException)
        {
            value = 0;
            return false;
        }
    }

    private static List<Token> Tokenize(string expression)
    {
        var tokens = new List<Token>();
        var i = 0;
        while (i < expression.Length)
        {
            var c = expression[i];
            if (char.IsWhiteSpace(c))
            {
                i++;
                continue;
            }

            if (char.IsDigit(c) || (c == '.' && i + 1 < expression.Length && char.IsDigit(expression[i + 1])))
            {
                var start = i;
                while (i < expression.Length && (char.IsDigit(expression[i]) || expression[i] == '.'))
                {
                    i++;
                }

                tokens.Add(new Token(TokenKind.Number, expression[start..i]));
                continue;
            }

            if (char.IsLetter(c) || c == '_')
            {
                var start = i;
                while (i < expression.Length && (char.IsLetterOrDigit(expression[i]) || expression[i] == '_'))
                {
                    i++;
                }

                tokens.Add(new Token(TokenKind.Identifier, expression[start..i]));
                continue;
            }

            if (c == '(' || c == ')' || c == '+' || c == '-' || c == '*' || c == '/' || c == '%')
            {
                tokens.Add(new Token(TokenKind.Symbol, c.ToString()));
                i++;
                continue;
            }

            throw new RuleExpressionException($"Unexpected character '{c}' at position {i}.");
        }

        return tokens;
    }

    private enum TokenKind
    {
        Number,
        Identifier,
        Symbol
    }

    private readonly record struct Token(TokenKind Kind, string Text);

    private sealed class Parser(List<Token> tokens, IReadOnlyDictionary<string, double> variables)
    {
        private int _position;

        public double ParseExpression()
        {
            var value = ParseTerm();
            while (CurrentIs("+") || CurrentIs("-"))
            {
                var op = Consume().Text;
                var right = ParseTerm();
                value = op == "+" ? value + right : value - right;
            }

            return value;
        }

        public void ExpectEnd()
        {
            if (_position < tokens.Count)
            {
                throw new RuleExpressionException($"Unexpected token '{tokens[_position].Text}'.");
            }
        }

        private double ParseTerm()
        {
            var value = ParseFactor();
            while (CurrentIs("*") || CurrentIs("/") || CurrentIs("%"))
            {
                var op = Consume().Text;
                var right = ParseFactor();
                value = op switch
                {
                    "*" => value * right,
                    "/" => right == 0 ? throw new RuleExpressionException("Division by zero.") : value / right,
                    _ => right == 0 ? throw new RuleExpressionException("Modulo by zero.") : value % right
                };
            }

            return value;
        }

        private double ParseFactor()
        {
            if (CurrentIs("-"))
            {
                Consume();
                return -ParseFactor();
            }

            if (CurrentIs("+"))
            {
                Consume();
                return ParseFactor();
            }

            return ParsePrimary();
        }

        private double ParsePrimary()
        {
            if (_position >= tokens.Count)
            {
                throw new RuleExpressionException("Unexpected end of expression.");
            }

            var token = Consume();
            if (token.Kind == TokenKind.Number)
            {
                if (!double.TryParse(token.Text, System.Globalization.CultureInfo.InvariantCulture, out var number))
                {
                    throw new RuleExpressionException($"Invalid number '{token.Text}'.");
                }

                return number;
            }

            if (token.Kind == TokenKind.Identifier)
            {
                if (_position < tokens.Count && tokens[_position].Text == "(")
                {
                    throw new RuleExpressionException($"Function calls are not allowed ('{token.Text}(').");
                }

                if (!variables.TryGetValue(token.Text, out var value))
                {
                    throw new RuleExpressionException($"Unknown variable '{token.Text}'.");
                }

                return value;
            }

            if (token.Text == "(")
            {
                var value = ParseExpression();
                if (_position >= tokens.Count || Consume().Text != ")")
                {
                    throw new RuleExpressionException("Missing closing parenthesis.");
                }

                return value;
            }

            throw new RuleExpressionException($"Unexpected token '{token.Text}'.");
        }

        private bool CurrentIs(string text) =>
            _position < tokens.Count && tokens[_position].Text == text;

        private Token Consume() => tokens[_position++];
    }
}
