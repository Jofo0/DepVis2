using DepVis.Shared.Model.Enums;

namespace DepVis.Shared.Messages;

public class UpdateProcessingMessage
{
    public required Guid ProjectBranchId { get; set; } = Guid.Empty;
    public required ProcessStatus ProcessStatus { get; set; }
    public string Branch { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
}
