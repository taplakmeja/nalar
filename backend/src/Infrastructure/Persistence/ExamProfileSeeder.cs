using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Psikotes.Domain.Entities;

namespace Psikotes.Infrastructure.Persistence;

public static class ExamProfileSeeder
{
    private static readonly IReadOnlyList<ExamProfile> Profiles =
    [
        new ExamProfile
        {
            Id = Guid.NewGuid(),
            Slug = "psikotes-umum",
            Name = "Psikotes Umum",
            Description = "Latihan umum untuk seleksi kerja: campuran seluruh kategori.",
            QuestionCount = 40,
            DurationMinutes = 60,
            PassingScorePercent = 60m
        },
        new ExamProfile
        {
            Id = Guid.NewGuid(),
            Slug = "cpns",
            Name = "CPNS",
            Description = "Simulasi seleksi CPNS dengan penekanan pada numerikal dan verbal.",
            QuestionCount = 50,
            DurationMinutes = 75,
            PassingScorePercent = 65m
        },
        new ExamProfile
        {
            Id = Guid.NewGuid(),
            Slug = "bumn",
            Name = "BUMN",
            Description = "Simulasi seleksi BUMN: akurasi tinggi dengan batas waktu ketat.",
            QuestionCount = 45,
            DurationMinutes = 60,
            PassingScorePercent = 65m
        },
        new ExamProfile
        {
            Id = Guid.NewGuid(),
            Slug = "tni-polri",
            Name = "TNI / Polri",
            Description = "Simulasi tes masuk TNI/Polri dengan fokus logika dan spasial.",
            QuestionCount = 50,
            DurationMinutes = 60,
            PassingScorePercent = 70m
        },
        new ExamProfile
        {
            Id = Guid.NewGuid(),
            Slug = "swasta",
            Name = "Rekrutmen Perusahaan Swasta",
            Description = "Format psikotes perusahaan swasta: seimbang dan ringkas.",
            QuestionCount = 30,
            DurationMinutes = 45,
            PassingScorePercent = 60m
        },
        new ExamProfile
        {
            Id = Guid.NewGuid(),
            Slug = "psikotes-umum-lengkap",
            Name = "Psikotes Umum Lengkap",
            Description = "Set soal lengkap tanpa tekanan waktu untuk latihan menyeluruh.",
            QuestionCount = 60,
            DurationMinutes = 90,
            PassingScorePercent = 60m
        }
    ];

    public static async Task SeedAsync(AppDbContext dbContext, ILogger logger)
    {
        var existingSlugs = await dbContext.ExamProfiles
            .Select(p => p.Slug)
            .ToListAsync();

        var missing = Profiles.Where(p => !existingSlugs.Contains(p.Slug)).ToList();
        if (missing.Count == 0)
        {
            return;
        }

        dbContext.ExamProfiles.AddRange(missing);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} exam profiles", missing.Count);
    }
}
