namespace DepVis.Shared.Messages;

public class ProcessingMessage
{
    public required string GitHubLink { get; set; } = string.Empty;
    public required string Branch { get; set; } = string.Empty;
    public required Guid ProjectId { get; set; } = Guid.Empty;
}
