using DepVis.Core.Dtos;
using DepVis.Shared.Model;
using Microsoft.AspNetCore.OData.Query;

namespace DepVis.Core.Services.Interfaces;

public interface IPackageService
{
    Task<PackagesDto> GetPackageData(Guid id, ODataQueryOptions<SbomPackage> odata, Guid? commitId = null);
    Task<PackageDetailedDto?> GetPackageData(Guid packageId, CancellationToken cancellation);
}

