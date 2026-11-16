using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Psikotes.Application.UseCases.Auth;

namespace Psikotes.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<AuthResponseFactory>();

        return services;
    }
}
