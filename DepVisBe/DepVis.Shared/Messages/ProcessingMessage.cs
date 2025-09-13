namespace DepVis.Shared.Messages;

public class ProcessingMessage
{
    public string GitHubLink { get; set; } = string.Empty;
    public Guid ProjectId { get; set; } = Guid.Empty;
}
