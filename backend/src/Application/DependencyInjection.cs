using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Psikotes.Application.Common.Interfaces;
using Psikotes.Application.Common.Services;
using Psikotes.Application.UseCases.Auth;
using Psikotes.QuestionEngine;
using Psikotes.QuestionEngine.Abstractions;
using Psikotes.QuestionEngine.Models;

namespace Psikotes.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<AuthResponseFactory>();

        services.AddSingleton<GeneratorRegistry>(sp =>
        {
            var generators = typeof(QuestionCategory).Assembly
                .GetTypes()
                .Where(t => t is { IsAbstract: false, IsInterface: false } && typeof(IQuestionGenerator).IsAssignableFrom(t))
                .Select(t => (IQuestionGenerator)Activator.CreateInstance(t)!)
                .ToList();

            return new GeneratorRegistry(generators);
        });
        services.AddScoped<IQuestionGenerationService, QuestionGenerationService>();
        services.AddScoped<ITrainingStatsService, TrainingStatsService>();
        services.AddSingleton<IAdaptiveDifficultyService, AdaptiveDifficultyService>();
        services.AddSingleton<IScoringService, ScoringService>();

        return services;
    }
}
