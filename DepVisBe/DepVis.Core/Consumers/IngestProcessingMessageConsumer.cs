using System.Text.Json;
using DepVis.Core.Context;
using DepVis.Shared.Messages;
using DepVis.Shared.Model;
using DepVis.Shared.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace DepVis.Core.Consumers;

public class IngestProcessingMessageConsumer(
    ILogger<IngestProcessingMessageConsumer> _logger,
    DepVisDbContext _db,
    MinioStorageService _minioStorageService
) : IConsumer<IngestProcessingMessage>
{
    public async Task Consume(ConsumeContext<IngestProcessingMessage> context)
    {
        var message = context.Message;

        _logger.LogDebug("Received IngestProcessingMessage for {sbomId}", message.SbomId);

        var sbom = await _db
            .Sboms.Include(x => x.Project)
            .FirstAsync(x => x.Id == message.SbomId, context.CancellationToken);

        var project = sbom.Project;

        project.ProcessStep = Shared.Model.Enums.ProcessStep.SbomIngest;
        project.ProcessStatus = Shared.Model.Enums.ProcessStatus.Pending;
        await _db.SaveChangesAsync();

        if (sbom == null)
        {
            _logger.LogWarning("Sbom with id {sbomId} not found", message.SbomId);
            project.ProcessStatus = Shared.Model.Enums.ProcessStatus.Failed;
            await _db.SaveChangesAsync();
            return;
        }

        var stream = await _minioStorageService.RetrieveAsync(
            sbom.FileName,
            context.CancellationToken
        );

        CycloneDxBom bom =
            JsonSerializer.Deserialize<CycloneDxBom>(
                stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new CycloneDxBom { Components = [] };
        var comps = bom.Components ?? [];
        var packages = new List<SbomPackage>(comps.Count + 1);
        var deps = bom.Dependencies ?? [];

        var rootProject = bom.Metadata.Root;

        packages.Add(
            new SbomPackage()
            {
                SbomId = sbom.Id,
                Name = "ProjectRoot",
                Version = null,
                Purl = null,
                Ecosystem = "",
                Type = "ProjectRoot",
                BomRef = rootProject.BomRef,
            }
        );

        packages.AddRange(
            comps.Select(x => new SbomPackage
            {
                SbomId = sbom.Id,
                Name = string.IsNullOrWhiteSpace(x.Name) ? "No Name Found" : x.Name,
                Version = string.IsNullOrWhiteSpace(x.Version) ? null : x.Version,
                Purl = string.IsNullOrWhiteSpace(x.Purl) ? null : x.Purl,
                Ecosystem = InferEcosystemFromPurl(x.Purl),
                Type = x.Type,
                BomRef = x.BomRef,
            })
        );

        var edges = deps.ToDictionary(d => d.Ref, d => d.DependsOn ?? []);

        var bomRefs = packages
            .Select(b => new { b.BomRef, b.Id })
            .ToDictionary(p => p.BomRef, p => p);

        var createdDeps = new HashSet<PackageDependency>();
        foreach (var (parent, children) in edges)
        {
            var parentPkg = bomRefs[parent];
            foreach (var child in children)
            {
                var childPkg = bomRefs[child];
                createdDeps.Add(
                    new PackageDependency { ParentId = parentPkg.Id, ChildId = childPkg.Id }
                );
            }
        }

        _db.SbomPackages.AddRange(packages);
        _db.PackageDependencies.AddRange(createdDeps);
        project.ProcessStatus = Shared.Model.Enums.ProcessStatus.Success;

        await _db.SaveChangesAsync(context.CancellationToken);

        _logger.LogDebug(
            "Successfully finished IngestProcessingMessage for {sbomId}",
            message.SbomId
        );
    }

    private static string? InferEcosystemFromPurl(string? purl)
    {
        if (string.IsNullOrWhiteSpace(purl))
            return null;

        var idx = purl.IndexOf(':');
        if (idx < 0)
            return null;
        var type = purl.Substring(4, (purl.IndexOf('/', idx + 1) - 4)).ToLowerInvariant();

        return type switch
        {
            "nuget" => "NuGet",
            "npm" => "npm",
            "maven" => "Maven",
            "pypi" => "PyPI",
            "golang" or "go" => "Go",
            "cargo" => "crates.io",
            "rubygems" => "RubyGems",
            "packagist" => "Packagist",
            _ => null,
        };
    }
}
