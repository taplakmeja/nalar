namespace Psikotes.Application.UseCases.Generators.Dtos;

public sealed record GeneratorConfigSummaryDto(
    Guid Id,
    string GeneratorId,
    int Version,
    string Name,
    string Category,
    string Status,
    DateTime CreatedAtUtc,
    DateTime? PublishedAtUtc,
    bool MassTestPassed);
