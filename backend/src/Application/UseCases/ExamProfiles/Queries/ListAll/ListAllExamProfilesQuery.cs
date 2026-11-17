using Ardalis.Result;
using Mediator;
using Psikotes.Application.UseCases.ExamProfiles.Dtos;

namespace Psikotes.Application.UseCases.ExamProfiles.Queries.ListAll;

public sealed record ListAllExamProfilesQuery : IQuery<Result<IReadOnlyList<AdminExamProfileDto>>>;
