namespace DepVis.Shared.Messages;

public class ProcessingMessage
{
    public required string GitHubLink { get; set; } = string.Empty;
    public required string Location { get; set; } = string.Empty;
    public required bool IsTag { get; set; } = false;
    public required Guid ProjectBranchId { get; set; } = Guid.Empty;
}
