using System.Text.Json;
using DepVis.Core.Services.Interfaces;
using DepVis.Shared.Model;
using DepVis.Shared.Services;

namespace DepVis.Core.Services.Processing;

public class CycloneDxBomLoader(MinioStorageService minio) : ICycloneDxBomLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
    };

    public async Task<CycloneDxBom> LoadAsync(
        string sbomFileName,
        CancellationToken cancellationToken = default
    )
    {
        await using var stream = await minio.RetrieveAsync(sbomFileName, cancellationToken);

        return JsonSerializer.Deserialize<CycloneDxBom>(stream, JsonOptions)
            ?? new CycloneDxBom { Components = [] };
    }
}
