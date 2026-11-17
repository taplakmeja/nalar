using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Psikotes.Application.UseCases.Ads.Dtos;
using Xunit;

namespace Psikotes.Web.IntegrationTests;

[Collection("Integration")]
public class AdsConfigTests
{
    private readonly PsikotesWebApplicationFactory _factory;

    public AdsConfigTests(PsikotesWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task AdConfig_WithoutSettings_ReturnsDisabledDefaults()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<Psikotes.Infrastructure.Persistence.AppDbContext>();
            var adsSettings = db.AppSettings.Where(s => s.Key.StartsWith("Ads."));
            db.AppSettings.RemoveRange(adsSettings);
            await db.SaveChangesAsync();
        }

        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/ads/config");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var config = await response.Content.ReadFromJsonAsync<AdConfigDto>();
        Assert.NotNull(config);
        Assert.False(config!.Enabled);
        Assert.Equal("adsense", config.Provider);
        Assert.Equal(string.Empty, config.Client);
        Assert.Equal("[]", config.PlacementsJson);
    }

    [Fact]
    public async Task AdConfig_WithEnabledSetting_ReturnsEnabled()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<Psikotes.Infrastructure.Persistence.AppDbContext>();
            db.AppSettings.Add(new Psikotes.Domain.Entities.AppSetting
            {
                Id = Guid.NewGuid(),
                Key = "Ads.Enabled",
                Value = "true",
                UpdatedAtUtc = DateTime.UtcNow
            });
            await db.SaveChangesAsync();
        }

        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/ads/config");
        response.EnsureSuccessStatusCode();

        var config = await response.Content.ReadFromJsonAsync<AdConfigDto>();
        Assert.NotNull(config);
        Assert.True(config!.Enabled);
    }
}
