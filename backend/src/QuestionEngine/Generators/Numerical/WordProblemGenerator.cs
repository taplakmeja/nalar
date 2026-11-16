using Psikotes.QuestionEngine.Abstractions;
using Psikotes.QuestionEngine.Models;
using Psikotes.QuestionEngine.Random;

namespace Psikotes.QuestionEngine.Generators.Numerical;

public sealed class WordProblemGenerator : IQuestionGenerator
{
    public GeneratorIdentity Identity { get; } = new("numerical.wordproblem", 1, QuestionCategory.Numerical);

    public GeneratedQuestion Generate(Difficulty difficulty, long seed)
    {
        var random = new SeededRandom(seed);
        var template = random.Next(3);
        string prompt;
        int answer;
        var parameters = new Dictionary<string, string>();

        switch (template)
        {
            case 0:
            {
                var price = random.Next(2, 10) * 500;
                var quantity = random.Next(2, 12);
                var paid = price * quantity + random.Next(1, 10) * 1000;
                answer = paid - price * quantity;
                prompt = $"Ani membeli {quantity} buku seharga {price:N0} rupiah per buku. Ia membayar {paid:N0} rupiah. Berapa rupiah kembalian yang diterima Ani?".Replace(",", ".");
                parameters["t"] = "change";
                break;
            }
            case 1:
            {
                var speed = random.Next(30, 90);
                var hours = random.Next(2, 7);
                answer = speed * hours;
                prompt = $"Sebuah bus melaju dengan kecepatan {speed} km/jam selama {hours} jam. Berapa kilometer jarak yang ditempuh bus tersebut?";
                parameters["t"] = "distance";
                break;
            }
            default:
            {
                var perWorker = random.Next(3, 9);
                var workers = random.Next(2, 9);
                answer = perWorker * workers;
                prompt = $"Setiap pekerja dapat merakit {perWorker} unit produk per hari. Berapa unit produk yang dirakit oleh {workers} pekerja dalam satu hari?";
                parameters["t"] = "work";
                break;
            }
        }

        var distractors = NumericDistractors.CreateStrings(answer, 3, random, maxOffset: System.Math.Max(4, answer / 5));
        var options = random.Shuffle([answer.ToString(), .. distractors]);

        return new GeneratedQuestion(
            Identity,
            difficulty,
            seed,
            parameters,
            prompt,
            answer.ToString(),
            options,
            ValidationStatus.Valid);
    }
}
