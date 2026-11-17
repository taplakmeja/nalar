using Psikotes.Domain.Enums;

namespace Psikotes.Domain.Entities;

/// <summary>
/// A versioned rule-based generator definition authored through the
/// Visual Generator Builder. Lifecycle: Draft → Published → RolledBack
/// (or Rejected). Only Drafts are editable; publishing requires a
/// passing mass test.
/// </summary>
public sealed class GeneratorConfig
{
    public Guid Id { get; set; }
    public string GeneratorId { get; set; } = string.Empty;
    public int Version { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string ConfigJson { get; set; } = string.Empty;
    public GeneratorConfigStatus Status { get; set; } = GeneratorConfigStatus.Draft;
    public string? CreatedBy { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? PublishedAtUtc { get; set; }
    public string? MassTestJson { get; set; }
    public string? ReviewNotes { get; set; }
}
