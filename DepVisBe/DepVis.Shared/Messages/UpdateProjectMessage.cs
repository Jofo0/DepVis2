using DepVis.Shared.Model.Enums;

namespace DepVis.Shared.Messages;

public class UpdateProjectMessage
{
    public Guid ProjectId { get; set; } = Guid.Empty;
    public ProcessStep ProcessStep { get; set; }
    public ProcessStatus ProcessStatus { get; set; }
}
