using System.Text.Json.Serialization;

namespace DepVis.Shared.Model;

public class CycloneDxBom
{
    [JsonPropertyName("components")]
    public List<CycloneDxComponent>? Components { get; set; }
}

public class CycloneDxComponent
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    [JsonPropertyName("purl")]
    public string Purl { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
}
