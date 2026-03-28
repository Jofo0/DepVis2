namespace DepVis.SbomProcessing.Models;

public sealed class CommitDescriptor
{
    public required string Sha { get; init; }
    public required string MessageShort { get; init; }
    public required DateTime CommitDate { get; init; }
}
