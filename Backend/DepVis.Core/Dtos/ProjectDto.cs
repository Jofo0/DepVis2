using DepVis.Shared.Model.Enums;

namespace DepVis.Core.Dtos;

public class ProjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ProjectLink { get; set; } = string.Empty;
    public string[] EcoSystems { get; set; } = [];
}
