using System.Text.Json.Serialization;

namespace DepVis.Shared.Model;

public class CycloneDxBom
{
    [JsonPropertyName("metadata")]
    public DxMetadata Metadata { get; set; }

    [JsonPropertyName("components")]
    public List<CycloneDxComponent>? Components { get; set; }

    [JsonPropertyName("dependencies")]
    public List<DepNode>? Dependencies { get; set; }

    [JsonPropertyName("vulnerabilities")]
    public List<CycloneDxVulnerability>? Vulnerabilities { get; set; }
}

public record CycloneDxComponent(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("version")] string Version,
    [property: JsonPropertyName("purl")] string Purl,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("bom-ref")] string BomRef
);

public record DepNode(
    [property: JsonPropertyName("ref")] string Ref,
    [property: JsonPropertyName("dependsOn")] List<string>? DependsOn
);

public record DxMetadata([property: JsonPropertyName("component")] CycloneDxComponent Root);

public record CycloneDxVulnerability(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("source")] Source Source,
    [property: JsonPropertyName("ratings")] List<Rating> Ratings,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("recommendation")] string Recommendation,
    [property: JsonPropertyName("advisories")] List<Advisory> Advisories,
    [property: JsonPropertyName("cwes")] List<long> CWEs,
    [property: JsonPropertyName("published")] DateTime Published,
    [property: JsonPropertyName("updated")] DateTime Updated,
    [property: JsonPropertyName("affects")] List<Affected> Affects
);

public record Source(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("url")] string? Url
);

public record Rating(
    [property: JsonPropertyName("source")] Source Source,
    [property: JsonPropertyName("score")] double? Score,
    [property: JsonPropertyName("severity")] string Severity,
    [property: JsonPropertyName("method")] string? Method,
    [property: JsonPropertyName("vector")] string? Vector
);

public record Advisory([property: JsonPropertyName("url")] string Url);

public record Affected(
    [property: JsonPropertyName("ref")] string Ref,
    [property: JsonPropertyName("versions")] List<VersionStatus> Versions
);

public record VersionStatus(
    [property: JsonPropertyName("version")] string Version,
    [property: JsonPropertyName("status")] string Status
);
