using Ardalis.Result;
using Mediator;
using Psikotes.Application.UseCases.Training.Dtos;

namespace Psikotes.Application.UseCases.Training.Queries.ListExamProfiles;

public sealed record ListExamProfilesQuery : IQuery<Result<IReadOnlyList<ExamProfileDto>>>;
