namespace DepVis.SbomProcessing.Models;

public sealed class ProgressState
{
    public int TotalCommits { get; init; }
    public int CommitsProcessed;
}
