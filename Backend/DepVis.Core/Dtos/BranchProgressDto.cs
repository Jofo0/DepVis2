namespace DepVis.Core.Dtos;

public class BranchProgressDto
{
    public int ProcessedCommits { get; set; }
    public int TotalCommits { get; set; }
    public int EstimatedSecondsRemaining { get; set; }
}