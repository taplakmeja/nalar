using Ardalis.Result;
using Mediator;
using Psikotes.Application.UseCases.Ads.Dtos;

namespace Psikotes.Application.UseCases.Ads.Queries.GetAdConfig;

public sealed record GetAdConfigQuery : IQuery<Result<AdConfigDto>>;
