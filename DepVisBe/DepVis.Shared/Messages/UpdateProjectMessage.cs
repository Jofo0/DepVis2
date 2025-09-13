using DepVis.Shared.Model.Enums;

namespace DepVis.Shared.Messages;

public class UpdateProjectMessage
{
    public required Guid ProjectId { get; set; } = Guid.Empty;
    public required ProcessStep ProcessStep { get; set; }
    public required ProcessStatus ProcessStatus { get; set; }
}
