namespace DepVis.Shared.Messages;

public class BranchProcessingCountMessage
{
    public required Guid ProjectBranchId { get; set; } = Guid.Empty;
    public int ProcessedCommits { get; set; } = 0;
    public int TotalCommits { get; set; } = 0;
}
