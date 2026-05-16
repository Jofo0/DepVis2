using DepVis.Core.Extensions;
using DepVis.Shared.Model;
using DepVis.Shared.Model.Enums;

namespace DepVis.Tests.Extensions;

public class DtoExtensionsTests
{
    [Fact]
    public void MapToDto_MapsProjectCorrectly()
    {
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = "TestProject",
            ProjectLink = "https://github.com/test",
            ProjectStatistics = new ProjectStatistics { EcoSystems = "nuget,npm,None" }
        };

        var dto = project.MapToDto();

        Assert.Equal(project.Id, dto.Id);
        Assert.Equal("TestProject", dto.Name);
        Assert.Equal(2, dto.EcoSystems.Length); // "None" filtered out
        Assert.Contains("nuget", dto.EcoSystems);
        Assert.Contains("npm", dto.EcoSystems);
    }

    [Fact]
    public void MapToDto_HandlesNullProjectStatistics()
    {
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            ProjectLink = "https://example.com",
            ProjectStatistics = null
        };

        var dto = project.MapToDto();

        Assert.Empty(dto.EcoSystems);
    }

    [Fact]
    public void MapToBranchesDto_CountsStepsCorrectly()
    {
        var branches = new List<ProjectBranch>
        {
            new() { ProcessStep = ProcessStep.Created, ProcessStatus = ProcessStatus.Success, BranchHistories = [] },
            new() { ProcessStep = ProcessStep.SbomCreation, ProcessStatus = ProcessStatus.Success, BranchHistories = [] },
            new() { ProcessStep = ProcessStep.SbomIngest, ProcessStatus = ProcessStatus.Success, BranchHistories = [] },
            new() { ProcessStep = ProcessStep.Processed, ProcessStatus = ProcessStatus.Success, BranchHistories = [] },
        };

        var dto = branches.MapToBranchesDto();

        Assert.Equal(4, dto.TotalCount);
        Assert.Equal(3, dto.Initiated);       // SbomCreation + SbomIngest + Processed
        Assert.Equal(3, dto.SbomGenerated);    // SbomCreation(Success) + SbomIngest + Processed
        Assert.Equal(2, dto.SbomIngested);     // SbomIngest(Success) + Processed
        Assert.Equal(1, dto.Complete);         // Processed with Success
    }

    [Fact]
    public void MapToPackageItemDto_SetsVulnerableFlag()
    {
        var vulnPkg = new SbomPackage
        {
            Name = "Vuln", Version = "1.0", Ecosystem = "nuget",
            Vulnerabilities = [new Vulnerability()]
        };
        var safePkg = new SbomPackage
        {
            Name = "Safe", Version = "1.0", Ecosystem = "nuget",
            Vulnerabilities = []
        };

        Assert.True(vulnPkg.MapToPackageItemDto().Vulnerable);
        Assert.False(safePkg.MapToPackageItemDto().Vulnerable);
    }

    [Fact]
    public void MapToBranchHistoryDto_OrdersByCommitDate()
    {
        var branch = new ProjectBranch
        {
            HistoryProcessingStep = ProcessStep.Processed,
            BranchHistories =
            [
                new BranchHistory { CommitDate = new DateTime(2025, 3, 1), CommitMessage = "Second" },
                new BranchHistory { CommitDate = new DateTime(2025, 1, 1), CommitMessage = "First" },
                new BranchHistory { CommitDate = new DateTime(2025, 5, 1), CommitMessage = "Third" },
            ]
        };

        var dto = branch.MapToBranchHistoryDto();

        Assert.Equal("First", dto.Histories[0].CommitMessage);
        Assert.Equal("Second", dto.Histories[1].CommitMessage);
        Assert.Equal("Third", dto.Histories[2].CommitMessage);
    }
}




