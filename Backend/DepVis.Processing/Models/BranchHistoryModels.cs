using DepVis.Shared.Messages;

namespace DepVis.SbomProcessing.Models;

public sealed class ProcessedCommitResult
{
    public required CommitProcessingInfo CommitInfo { get; init; }
    public required long PackageCount { get; init; }
    public required long VulnerabilityCount { get; init; }
}
