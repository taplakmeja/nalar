using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Psikotes.Domain.Common;
using Psikotes.Domain.Entities;

namespace Psikotes.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
        ILogger logger)
    {
        foreach (var role in Roles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var email = configuration["SeedAdmin:Email"];
        var password = configuration["SeedAdmin:Password"];

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            return;
        }

        if (await userManager.FindByEmailAsync(email) is null)
        {
            var admin = new ApplicationUser
            {
                UserName = email,
                Email = email,
                DisplayName = "Administrator",
                CreatedAtUtc = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(admin, password);
            if (result.Succeeded)
            {
                await userManager.AddToRolesAsync(admin, [Roles.Administrator, Roles.ContentManager]);
                logger.LogInformation("Seed admin account created for {Email}", email);
            }
            else
            {
                logger.LogWarning("Seed admin creation failed: {Errors}",
                    string.Join(" ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}
