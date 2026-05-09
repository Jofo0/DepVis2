using DepVis.Core.Dtos;

namespace DepVis.Core.Services.Interfaces;

public interface IGraphService
{
    Task<GraphDataDto?> GetProjectGraphData(Guid branchId, bool showAllParents = true, string? severityFilter = null, Guid? commitId = null);
    Task<GraphDataDto?> GetPackageHierarchyGraphData(Guid branchId, Guid packageId, Guid? commitId = null);
}

