namespace DepVis.Shared.Messages;

public class BranchHistoryProcessingMessage
{
    public required string GitHubLink { get; set; } = string.Empty;
    public required string Location { get; set; } = string.Empty;
    public required Guid ProjectBranchId { get; set; } = Guid.Empty;
}
