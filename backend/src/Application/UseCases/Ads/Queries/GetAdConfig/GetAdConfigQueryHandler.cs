using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Psikotes.Application.Common.Interfaces;
using Psikotes.Application.UseCases.Ads.Dtos;

namespace Psikotes.Application.UseCases.Ads.Queries.GetAdConfig;

public sealed class GetAdConfigQueryHandler(IAppDbContext dbContext)
    : IQueryHandler<GetAdConfigQuery, Result<AdConfigDto>>
{
    public async ValueTask<Result<AdConfigDto>> Handle(GetAdConfigQuery query, CancellationToken cancellationToken)
    {
        var settings = await dbContext.AppSettings
            .AsNoTracking()
            .Where(s => s.Key.StartsWith("Ads."))
            .ToDictionaryAsync(s => s.Key, s => s.Value, cancellationToken);

        return Result.Success(new AdConfigDto(
            Enabled: settings.TryGetValue("Ads.Enabled", out var enabled) && enabled == "true",
            Provider: settings.GetValueOrDefault("Ads.Provider", "adsense"),
            Client: settings.GetValueOrDefault("Ads.Client", string.Empty),
            PlacementsJson: settings.GetValueOrDefault("Ads.PlacementsJson", "[]")));
    }
}
