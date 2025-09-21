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

        var sbom = await db.Sboms.FirstAsync(
            x => x.Id == message.SbomId,
            context.CancellationToken
        );

        if (sbom == null)
        {
            logger.LogWarning("Sbom with id {sbomId} not found", message.SbomId);
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
            ) ?? new CycloneDxBom { Components = new() };

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
                    Name = c.Name,
                    Version = string.IsNullOrWhiteSpace(c.Version) ? null : c.Version,
                    Purl = string.IsNullOrWhiteSpace(c.Purl) ? null : c.Purl,
                    Ecosystem = InferEcosystemFromPurl(c.Purl),
                    Type = c.Type,
                }
            );
        }

        db.SbomPackages.AddRange(packages);
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
