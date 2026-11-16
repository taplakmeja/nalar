namespace Psikotes.QuestionEngine.Generators.Kraepelin;

public interface IKraepelinGenerator
{
    KraepelinGrid Generate(int rows, int columns, long seed, int durationMinutes);
}
