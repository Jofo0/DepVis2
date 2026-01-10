namespace DepVis.Shared.Messages;

public class IngestProcessingMessage
{
    public required Guid SbomId { get; set; }
    public bool IsHistory { get; set; } = false;
}
