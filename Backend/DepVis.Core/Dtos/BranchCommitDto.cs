using DepVis.Shared.Model.Enums;

namespace DepVis.Core.Dtos;

public class BranchCommitDto
{
    public string CommitName { get; set; } = string.Empty;
    public Guid CommitId { get; set; }
    public HistoryProcessing ProcessState { get; set; }
    public ProcessStatus ProcessStatus { get; set; }
}
