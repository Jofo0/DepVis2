using DepVis.Core.Dtos;
using DepVis.Shared.Model;

namespace DepVis.Core.Extensions;

public static class DtoExtensions
{
    public static ProjectDto MapToDto(this Project project) =>
        new()
        {
            Id = project.Id,
            Name = project.Name,
            ProjectType = project.ProjectType,
            ProcessStatus = project.ProcessStatus,
            ProcessStep = project.ProcessStep,
            ProjectLink = project.ProjectLink,
        };
}
