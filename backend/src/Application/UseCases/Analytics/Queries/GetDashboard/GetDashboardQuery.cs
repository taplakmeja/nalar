using Ardalis.Result;
using Mediator;
using Psikotes.Application.UseCases.Analytics.Dtos;

namespace Psikotes.Application.UseCases.Analytics.Queries.GetDashboard;

public sealed record GetDashboardQuery : IQuery<Result<DashboardDto>>;
