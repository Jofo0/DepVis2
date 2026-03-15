using DepVis.Shared.Model;

namespace DepVis.Core.Services.Processing;

public class SbomPackageBuilder : ISbomPackageBuilder
{
    public PackageBuildResult Build(Guid sbomId, CycloneDxBom bom)
    {
        var existingPackages = new Dictionary<string, PackagesDuplicatesResolve>();
        var components = bom.Components ?? [];
        var packages = new List<SbomPackage>(components.Count + 1);

        var rootRef = bom.Metadata?.Root?.BomRef ?? "project-root";

        packages.Add(
            new SbomPackage
            {
                Id = Guid.NewGuid(),
                SbomId = sbomId,
                Name = "ProjectRoot",
                Version = null,
                Purl = null,
                Ecosystem = "None",
                Type = "ProjectRoot",
                BomRef = rootRef,
                Depth = 0,
            }
        );

        foreach (var component in components)
        {
            if (component is null)
                continue;

            var packageId = Guid.NewGuid();

            if (!string.IsNullOrEmpty(component.Purl))
            {
                if (existingPackages.TryGetValue(component.Purl, out var existing))
                {
                    if (!string.IsNullOrWhiteSpace(component.BomRef))
                    {
                        existing.BomRefs.Add(component.BomRef);
                    }

                    continue;
                }

                existingPackages[component.Purl] = new PackagesDuplicatesResolve([], packageId);
            }

            var groupPrefix = string.IsNullOrWhiteSpace(component.Group)
                ? string.Empty
                : component.Group + "/";

            var packageName = string.IsNullOrWhiteSpace(component.Name)
                ? "No Name Found"
                : component.Name;

            var packageType = GetPackageTypeFromProperties(component.Properties);

            packages.Add(
                new SbomPackage
                {
                    Id = packageId,
                    SbomId = sbomId,
                    Name = groupPrefix + packageName,
                    Version = string.IsNullOrWhiteSpace(component.Version)
                        ? null
                        : component.Version,
                    Purl = string.IsNullOrWhiteSpace(component.Purl) ? null : component.Purl,
                    PackageType = packageType,
                    Ecosystem = InferEcosystem(component.Purl, packageType),
                    Type = component.Type,
                    BomRef = component.BomRef,
                }
            );
        }

        return new PackageBuildResult(packages, [.. existingPackages.Values], rootRef);
    }

    private static string? GetPackageTypeFromProperties(List<CycloneDxProperty> props)
    {
        var packageType = props.FirstOrDefault(p => p.Name == "aquasecurity:trivy:PkgType");

        return packageType?.Value ?? "None";
    }

    private static string? InferEcosystem(string? purl, string? packageType)
    {
        var fallback = string.IsNullOrEmpty(packageType) ? "None" : packageType;

        if (string.IsNullOrWhiteSpace(purl))
            return fallback;

        const string prefix = "pkg:";
        if (!purl.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            return fallback;

        var slashIndex = purl.IndexOf('/', prefix.Length);
        if (slashIndex < 0)
            return fallback;

        return purl[prefix.Length..slashIndex].ToLowerInvariant();
    }
}
