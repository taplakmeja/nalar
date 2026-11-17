using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Psikotes.Application.Common.Interfaces;
using Psikotes.Domain.Entities;

namespace Psikotes.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole, string>(options), IAppDbContext
{
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<TrainingSession> TrainingSessions => Set<TrainingSession>();
    public DbSet<SessionQuestion> SessionQuestions => Set<SessionQuestion>();
    public DbSet<ExamProfile> ExamProfiles => Set<ExamProfile>();
    public DbSet<GeneratorConfig> GeneratorConfigs => Set<GeneratorConfig>();
    public DbSet<AppSetting> AppSettings => Set<AppSetting>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Token).HasMaxLength(200);
            entity.HasIndex(t => t.Token).IsUnique();
            entity.HasIndex(t => t.UserId);
        });

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(u => u.DisplayName).HasMaxLength(100);
        });

        builder.Entity<TrainingSession>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Mode).HasConversion<string>().HasMaxLength(30);
            entity.Property(s => s.CategoryScope).HasConversion<string>().HasMaxLength(30);
            entity.Property(s => s.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(s => s.Score).HasPrecision(6, 2);
            entity.HasIndex(s => s.UserId);
            entity.HasOne(s => s.ExamProfile)
                .WithMany()
                .HasForeignKey(s => s.ExamProfileId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasMany(s => s.Questions)
                .WithOne(q => q.Session)
                .HasForeignKey(q => q.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<SessionQuestion>(entity =>
        {
            entity.HasKey(q => q.Id);
            entity.Property(q => q.Category).HasMaxLength(30);
            entity.Property(q => q.GeneratorId).HasMaxLength(100);
            entity.Property(q => q.ParametersJson).HasMaxLength(2000);
            entity.Property(q => q.CorrectAnswer).HasMaxLength(500);
            entity.Property(q => q.OptionsJson).HasMaxLength(4000);
            entity.Property(q => q.SubmittedAnswer).HasMaxLength(500);
            entity.HasIndex(q => q.SessionId);
        });

        builder.Entity<AppSetting>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Key).HasMaxLength(100);
            entity.HasIndex(s => s.Key).IsUnique();
            entity.Property(s => s.Value).HasMaxLength(2000);
        });

        builder.Entity<GeneratorConfig>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.GeneratorId).HasMaxLength(100);
            entity.Property(c => c.Name).HasMaxLength(100);
            entity.Property(c => c.Category).HasMaxLength(30);
            entity.Property(c => c.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(c => c.ConfigJson).HasMaxLength(20000);
            entity.Property(c => c.MassTestJson).HasMaxLength(4000);
            entity.Property(c => c.ReviewNotes).HasMaxLength(1000);
            entity.HasIndex(c => new { c.GeneratorId, c.Version }).IsUnique();
        });

        builder.Entity<ExamProfile>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Slug).HasMaxLength(50);
            entity.HasIndex(p => p.Slug).IsUnique();
            entity.Property(p => p.Name).HasMaxLength(100);
            entity.Property(p => p.Description).HasMaxLength(500);
            entity.Property(p => p.CategoryWeightsJson).HasMaxLength(2000);
        });
    }
}
