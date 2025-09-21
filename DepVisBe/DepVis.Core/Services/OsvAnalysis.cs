//using System.Collections.Concurrent;
//using OSV.Client;
//using OSV.Schema;
//using Query = OSV.Client.Models.Query;

//public interface IVulnerabilityScanner
//{
//    Task<IReadOnlyList<PackageVulns>> ScanAsync(SbomBom bom, CancellationToken ct = default);
//}

//public record PackageVulns(string PackageKey, IReadOnlyList<string> VulnerabilityIds);

//public class OsvVulnerabilityScanner(OSVClient osv, ILogger<OsvVulnerabilityScanner> logger)
//    : IVulnerabilityScanner
//{
//    public async Task<IReadOnlyList<PackageVulns>> ScanAsync(
//        SbomBom bom,
//        CancellationToken ct = default
//    )
//    {
//        var results = new ConcurrentBag<PackageVulns>();
//        var components = bom.Components ?? [];

//        using var gate = new SemaphoreSlim(10);

//        await Task.WhenAll(
//            components
//                .Where(c =>
//                    !string.IsNullOrWhiteSpace(c.Purl) || !string.IsNullOrWhiteSpace(c.Name)
//                )
//                .Select(async c =>
//                {
//                    await gate.WaitAsync(ct);
//                    try
//                    {
//                        var q = BuildQuery(c);
//                        var resp = await osv.QueryAffectedAsync(q, ct); // from osv.net
//                        var ids =
//                            resp.Vulnerabilities?.Select(v => v.Id).ToArray()
//                            ?? Array.Empty<string>();

//                        var key = c.Purl ?? $"{c.Name}@{c.Version}";
//                        results.Add(new PackageVulns(key, ids));
//                    }
//                    catch (OSVException ex)
//                    {
//                        logger.LogWarning(ex, "OSV lookup failed for {Pkg}", c.Purl ?? c.Name);
//                    }
//                    finally
//                    {
//                        gate.Release();
//                    }
//                })
//        );

//        return results.ToList();
//    }

//    private static Query BuildQuery(SbomComponent c)
//    {
//        if (!string.IsNullOrWhiteSpace(c.Purl))
//        {
//            // If the PURL already has '@version', leave Version = null (OSV rule)
//            var hasVersion = c.Purl.Contains('@');
//            return new Query
//            {
//                Package = new Package { Purl = c.Purl },
//                Version = hasVersion ? null : c.Version,
//            };
//        }

//        // No purl? Use name + ecosystem + version
//        return new Query
//        {
//            Package = new Package
//            {
//                Name = c.Name,
//                Ecosystem = InferEcosystemFromYourData(c), // e.g., "NuGet", "npm", "Maven", "PyPI", "crates.io", etc.
//            },
//            Version = c.Version,
//        };
//    }

//    private static string? InferEcosystemFromYourData(SbomComponent c)
//    {
//        // If you can parse PURL type, map it:
//        // nuget -> "NuGet", npm -> "npm", maven -> "Maven", pypi -> "PyPI", golang -> "Go", cargo -> "crates.io", rubygems -> "RubyGems", packagist -> "Packagist"
//        return null;
//    }
//}
