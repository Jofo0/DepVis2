using DepVis.Core.Dtos;
using DepVis.Shared.Model;
using Microsoft.AspNetCore.OData.Query;

namespace DepVis.Core.Services.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectDto>> GetProjects();
    Task<ProjectDto?> GetProject(Guid id);
    Task<List<ProjectBranchDetailedDto>> GetProjectBranchesDetailed(
        Guid id,
        ODataQueryOptions<ProjectBranch> odata
    );
    Task<ProjectDto> CreateProject(CreateProjectDto dto);
    Task<PackagesDto> GetPackageData(Guid id, ODataQueryOptions<SbomPackage> odata);
    Task<VulnerabilitiesDto> GetVulnerabilities(
        Guid branchId,
        ODataQueryOptions<VulnerabilitySmallDto> odata
    );
    Task<GraphDataDto?> GetPackageHierarchyGraphData(Guid branchId, Guid packageId);
    Task<VulnerabilityDetailedDto?> GetVulnerability(string vulnId);
    Task<bool> UpdateProject(Guid id, UpdateProjectDto dto);
    Task<ProjectStatsDto> GetProjectStats(Guid branchId);
    Task<List<ProjectBranchDto>> GetProjectBranches(Guid id);
    Task<GraphDataDto> GetProjectGraphData(Guid branchId);
    Task<bool> DeleteProject(Guid id);
}
