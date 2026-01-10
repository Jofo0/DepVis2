using DepVis.Shared.Model.Enums;

namespace DepVis.Shared.Messages;

public class UpdateBranchHistoryProcessingMessage
{
    public required Guid ProjectBranchId { get; set; } = Guid.Empty;
    public required ProcessStatus ProcessStatus { get; set; }
    public required ProcessStep ProcessStep { get; set; }
    public string Branch { get; set; } = string.Empty;
    public List<CommitProcessingInfo> Commits { get; set; } = [];
}

public class CommitProcessingInfo
{
    public string FileName { get; set; } = string.Empty;
    public string CommitMessage { get; set; } = string.Empty;
    public string CommitSha { get; set; } = string.Empty;
    public DateTime CommitDate { get; set; }
}
