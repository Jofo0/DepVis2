using System.Text.Json;
using DepVis.Core.Context;
using DepVis.Shared.Messages;
using DepVis.Shared.Model;
using DepVis.Shared.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace DepVis.Core.Consumers;

public class IngestProcessingMessageConsumer(
    ILogger<IngestProcessingMessageConsumer> logger,
    DepVisDbContext db,
    MinioStorageService minioStorageService
) : IConsumer<IngestProcessingMessage>
{
    public async Task Consume(ConsumeContext<IngestProcessingMessage> context)
    {
        var message = context.Message;

        var sbom = await db
            .Sboms.Include(x => x.Project)
            .FirstAsync(x => x.Id == message.SbomId, context.CancellationToken);

        var project = sbom.Project;

        project.ProcessStep = Shared.Model.Enums.ProcessStep.SbomIngest;
        project.ProcessStatus = Shared.Model.Enums.ProcessStatus.Pending;
        await db.SaveChangesAsync();

        if (sbom == null)
        {
            logger.LogWarning("Sbom with id {sbomId} not found", message.SbomId);
            project.ProcessStatus = Shared.Model.Enums.ProcessStatus.Failed;
            await db.SaveChangesAsync();
            return;
        }

        var stream = await minioStorageService.RetrieveAsync(
            sbom.FileName,
            context.CancellationToken
        );

        if (stream.CanSeek)
            stream.Seek(0, SeekOrigin.Begin);

        CycloneDxBom bom =
            await JsonSerializer.DeserializeAsync<CycloneDxBom>(
                stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                context.CancellationToken
            ) ?? new CycloneDxBom { Components = [] };

        var comps = bom.Components ?? [];
        var packages = new List<SbomPackage>(comps.Count);
        var deps = bom.Dependencies ?? [];

        packages.AddRange(
            comps
                .Where(x => !string.IsNullOrWhiteSpace(x.Name))
                .Select(x => new SbomPackage
                {
                    SbomId = sbom.Id,
                    Name = x.Name,
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

        var createdDeps = new List<PackageDependency>();
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

        db.SbomPackages.AddRange(packages);
        db.PackageDependencies.AddRange(createdDeps);
        project.ProcessStatus = Shared.Model.Enums.ProcessStatus.Success;

        await db.SaveChangesAsync(context.CancellationToken);
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
