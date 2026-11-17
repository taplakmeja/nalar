namespace Psikotes.Application.UseCases.Generators.Dtos;

public sealed record GeneratorConfigDto(
    Guid Id,
    string GeneratorId,
    int Version,
    string Name,
    string Category,
    string Status,
    string ConfigJson,
    DateTime CreatedAtUtc,
    DateTime? PublishedAtUtc,
    string? MassTestJson,
    string? ReviewNotes);
