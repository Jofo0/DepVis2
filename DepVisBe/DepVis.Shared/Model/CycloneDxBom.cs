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
