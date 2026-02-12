using DepVis.Shared.Model.Enums;

namespace DepVis.Core.Dtos;

public class EditProjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ProjectLink { get; set; } = string.Empty;
    public List<string> Branches { get; set; } = [];
    public List<string> Tags { get; set; } = [];
}
