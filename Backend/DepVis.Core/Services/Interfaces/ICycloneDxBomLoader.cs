using DepVis.Shared.Model;

namespace DepVis.Core.Services.Interfaces;

public interface ICycloneDxBomLoader
{
    Task<CycloneDxBom> LoadAsync(
        string sbomFileName,
        CancellationToken cancellationToken = default
    );
}
