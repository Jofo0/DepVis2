namespace DepVis.SbomProcessing.Models;

public sealed class ProgressState
{
    public int CommitsProcessed;
    public long StartedAtTicks = DateTime.UtcNow.Ticks;
    public int TotalCommits { get; init; }
}