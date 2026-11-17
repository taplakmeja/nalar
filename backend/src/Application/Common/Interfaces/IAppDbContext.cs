using Microsoft.EntityFrameworkCore;
using Psikotes.Domain.Entities;

namespace Psikotes.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<ApplicationUser> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<TrainingSession> TrainingSessions { get; }
    DbSet<SessionQuestion> SessionQuestions { get; }
    DbSet<ExamProfile> ExamProfiles { get; }
    DbSet<GeneratorConfig> GeneratorConfigs { get; }
    DbSet<AppSetting> AppSettings { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
