using System.Text.Json;
using DepVis.Shared.Model;
using DepVis.Shared.Services;

namespace DepVis.Core.Services;

public class SbomIngestService(MinioStorageService minioStorageService)
{
    public async Task IngestAsync(Sbom sbom, CancellationToken ct = default)
    {
        CycloneDxBom bom;
        var stream = await minioStorageService.RetrieveAsync(sbom.FileName);

        await using (var fs = File.OpenRead(sbomJsonPath))
        {
            bom =
                await JsonSerializer.DeserializeAsync<CycloneDxBom>(
                    fs,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                    ct
                ) ?? new CycloneDxBom { Components = new() };
        }

        var comps = bom.Components ?? new();
        var packages = new List<SbomPackage>(comps.Count);

        foreach (var c in comps)
        {
            if (string.IsNullOrWhiteSpace(c?.Name))
                continue;

            packages.Add(
                new SbomPackage
                {
                    SbomId = sbom.Id,
                    Name = c.Name!,
                    Version = string.IsNullOrWhiteSpace(c.Version) ? null : c.Version,
                    Purl = string.IsNullOrWhiteSpace(c.Purl) ? null : c.Purl,
                    Ecosystem = InferEcosystemFromPurl(c.Purl),
                    Type = c.Type,
                    Group = c.Group,
                }
            );
        }

        db.SbomPackages.AddRange(packages);
        await db.SaveChangesAsync(ct);
    }
}
