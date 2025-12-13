namespace DepVis.Core.Dtos;

public class GraphDataDto
{
    public List<PackageDto> Packages { get; set; }
    public List<PackageRelationDto> Relationships { get; set; }
}

public class PackageDto
{
    public string Name { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;

    public Guid Id { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is PackageDto dto && Name == dto.Name && Id.Equals(dto.Id);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Id);
    }
}

public class PackageRelationDto
{
    public Guid From { get; set; }
    public Guid To { get; set; }
}
