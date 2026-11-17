using Ardalis.Result;
using Mediator;

namespace Psikotes.Application.UseCases.ExamProfiles.Commands.Delete;

public sealed record DeleteExamProfileCommand(Guid Id) : ICommand<Result>;
