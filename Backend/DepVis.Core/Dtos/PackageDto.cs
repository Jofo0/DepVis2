namespace DepVis.Core.Dtos;

public class PackagesDto
{
    public List<PackageItemDto> PackageItems { get; set; } = [];
    public List<NameCount> Vulnerabilities { get; set; } = [];
    public List<NameCount> EcoSystems { get; set; } = [];
}

public class PackageItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Ecosystem { get; set; } = string.Empty;
    public bool Vulnerable { get; set; } = false;
}
