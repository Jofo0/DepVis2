namespace DepVis.Core.Dtos;

public class GraphDataDto
{
    public List<PackageDto> Packages { get; set; }
    public List<PackageRelationDto> Relationships { get; set; }
}

public class PackageDto
{
    public string Name { get; set; }
    public Guid Id { get; set; }
}

public class PackageRelationDto
{
    public Guid From { get; set; }
    public Guid To { get; set; }
}
