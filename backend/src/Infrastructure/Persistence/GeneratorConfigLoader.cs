using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Psikotes.Application.Common.Interfaces;
using Psikotes.Domain.Enums;
using Psikotes.QuestionEngine;

namespace Psikotes.Infrastructure.Persistence;

public sealed class GeneratorConfigLoader
{
    public static async Task LoadAsync(
        AppDbContext dbContext,
        GeneratorRegistry registry,
        IRuleGeneratorFactory factory,
        ILogger logger)
    {
        var published = await dbContext.GeneratorConfigs
            .AsNoTracking()
            .Where(c => c.Status == GeneratorConfigStatus.Published)
            .OrderBy(c => c.GeneratorId)
            .ThenBy(c => c.Version)
            .ToListAsync();

        foreach (var config in published)
        {
            var generator = factory.Create(
                config.GeneratorId,
                config.Version,
                config.Category,
                config.ConfigJson,
                out var issues);

            if (issues.Count > 0)
            {
                logger.LogWarning(
                    "Skipping published generator {GeneratorId} v{Version}: {Issues}",
                    config.GeneratorId,
                    config.Version,
                    string.Join(" ", issues));
                continue;
            }

            registry.Activate(generator);
            logger.LogInformation(
                "Activated published generator {GeneratorId} v{Version}",
                config.GeneratorId,
                config.Version);
        }
    }
}
