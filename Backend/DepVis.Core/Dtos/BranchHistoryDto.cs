using DepVis.Shared.Model.Enums;

namespace DepVis.Core.Dtos;

public class BranchHistoryDto
{
    public ProcessStep ProcessingStep { get; set; }
    public List<BranchHistoryEntryDto> Histories { get; set; } = [];
}

public class BranchHistoryEntryDto
{
    public DateTime CommitDate { get; set; }
    public string CommitMessage { get; set; } = string.Empty;
    public string CommitSha { get; set; } = string.Empty;
    public int PackageCount { get; set; }
    public int VulnerabilityCount { get; set; }
}
