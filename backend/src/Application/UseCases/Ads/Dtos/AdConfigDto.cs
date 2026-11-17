namespace Psikotes.Application.UseCases.Ads.Dtos;

public sealed record AdConfigDto(
    bool Enabled,
    string Provider,
    string Client,
    string PlacementsJson);
