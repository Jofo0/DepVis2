using DepVis.Core.Dtos;
using DepVis.Core.Repositories.Interfaces;
using DepVis.Core.Services;
using DepVis.Shared.Messages;
using DepVis.Shared.Model;
using DepVis.Shared.Model.Enums;
using MassTransit;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;

namespace DepVis.Tests.Services;

public class ProjectBranchServiceTests
{
    private readonly IProjectBranchRepository _repo = Substitute.For<IProjectBranchRepository>();
    private readonly ISbomRepository _sbomRepo = Substitute.For<ISbomRepository>();
    private readonly IPublishEndpoint _publishEndpoint = Substitute.For<IPublishEndpoint>();
    private readonly IMemoryCache _cache;
    private readonly ProjectBranchService _sut;

    public ProjectBranchServiceTests()
    {
        _cache = new MemoryCache(new MemoryCacheOptions());
        _sut = new ProjectBranchService(_repo, _sbomRepo, _publishEndpoint, _cache);
    }

    [Fact]
    public async Task ProcessBranch_DoesNothing_WhenBranchNotFound()
    {
        _repo.GetByIdAsync(Arg.Any<Guid>()).Returns((ProjectBranch?)null);

        await _sut.ProcessBranch(Guid.NewGuid());

        await _publishEndpoint.DidNotReceive().Publish(Arg.Any<ProcessingMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ProcessBranch_PublishesMessage_WhenBranchExists()
    {
        var branch = new ProjectBranch
        {
            Id = Guid.NewGuid(),
            Name = "main",
            Project = new Project { ProjectLink = "https://github.com/test/repo" }
        };
        _repo.GetByIdAsync(branch.Id).Returns(branch);

        await _sut.ProcessBranch(branch.Id);

        await _publishEndpoint.Received(1).Publish(Arg.Is<ProcessingMessage>(m =>
            m.GitHubLink == "https://github.com/test/repo" &&
            m.GitTarget.Location == "main"
        ), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetComparison_DetectsAddedAndRemovedPackages()
    {
        var mainId = Guid.NewGuid();
        var compId = Guid.NewGuid();

        _repo.GetCompareDataAsync(mainId).Returns(new BranchCompareDataModel(
            mainId,
            [new SmallPackage("PkgA", "1.0", "nuget"), new SmallPackage("PkgB", "1.0", "npm")],
            ["CVE-1"]
        ));

        _repo.GetCompareDataAsync(compId).Returns(new BranchCompareDataModel(
            compId,
            [new SmallPackage("PkgA", "1.0", "nuget"), new SmallPackage("PkgC", "2.0", "nuget")],
            ["CVE-2"]
        ));

        var result = await _sut.GetComparison(mainId, compId);

        Assert.Single(result.AddedPackages); // PkgC added
        Assert.Equal("PkgC", result.AddedPackages[0].Name);
        Assert.Single(result.RemovedPackages); // PkgB removed
        Assert.Equal("PkgB", result.RemovedPackages[0].Name);
        Assert.Single(result.AddedVulnerabilityIds); // CVE-2 added
        Assert.Single(result.RemovedVulnerabilityIds); // CVE-1 removed
        Assert.Equal(2, result.mainBranchPackageCount);
        Assert.Equal(2, result.comparedBranchPackageCount);
    }

    [Fact]
    public async Task GetComparison_DetectsVersionChange_AsBothAddedAndRemoved()
    {
        var mainId = Guid.NewGuid();
        var compId = Guid.NewGuid();

        _repo.GetCompareDataAsync(mainId).Returns(new BranchCompareDataModel(
            mainId, [new SmallPackage("Pkg", "1.0", "nuget")], []
        ));
        _repo.GetCompareDataAsync(compId).Returns(new BranchCompareDataModel(
            compId, [new SmallPackage("Pkg", "2.0", "nuget")], []
        ));

        var result = await _sut.GetComparison(mainId, compId);

        // SmallPackage equality is Name+Version, so version change = removed old + added new
        Assert.Single(result.AddedPackages);
        Assert.Equal("2.0", result.AddedPackages[0].Version);
        Assert.Single(result.RemovedPackages);
        Assert.Equal("1.0", result.RemovedPackages[0].Version);
    }

    [Fact]
    public void GetBranchProgress_ReturnsNull_WhenNotCached()
    {
        var result = _sut.GetBranchProgress(Guid.NewGuid());
        Assert.Null(result);
    }

    [Fact]
    public void GetBranchProgress_ReturnsCachedValue()
    {
        var branchId = Guid.NewGuid();
        var dto = new BranchProgressDto { ProcessedCommits = 5, TotalCommits = 10, EstimatedSecondsRemaining = 30 };
        _cache.Set($"branch-progress:{branchId}", dto);

        var result = _sut.GetBranchProgress(branchId);

        Assert.NotNull(result);
        Assert.Equal(5, result.ProcessedCommits);
        Assert.Equal(10, result.TotalCommits);
        Assert.Equal(30, result.EstimatedSecondsRemaining);
    }

    [Fact]
    public async Task GetBranchHistory_AttachesProgress_WhenInSbomCreationStep()
    {
        var branchId = Guid.NewGuid();
        var branch = new ProjectBranch
        {
            Id = branchId,
            HistoryProcessingStep = ProcessStep.SbomCreation,
            BranchHistories = []
        };
        _repo.GetProjectBranchHistory(branchId, Arg.Any<CancellationToken>()).Returns(branch);

        var progress = new BranchProgressDto { ProcessedCommits = 3, TotalCommits = 10, EstimatedSecondsRemaining = 42 };
        _cache.Set($"branch-progress:{branchId}", progress);

        var result = await _sut.GetBranchHistory(branchId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(3, result.ProcessedCommits);
        Assert.Equal(10, result.TotalCommits);
        Assert.Equal(42, result.EstimatedSecondsRemaining);
    }

    [Fact]
    public async Task IngestHistory_DoesNothing_WhenHistoryNotFound()
    {
        _repo.GetBranchHistoryAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((BranchHistory?)null);

        await _sut.IngestHistory(Guid.NewGuid(), CancellationToken.None);

        await _publishEndpoint.DidNotReceive().Publish(Arg.Any<IngestBranchHistoryMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetLatestSbomForBranch_DelegatesToRepository()
    {
        var branchId = Guid.NewGuid();
        var sbom = new Sbom { Id = Guid.NewGuid() };
        _sbomRepo.GetLatestByBranchIdAsync(branchId).Returns(sbom);

        var result = await _sut.GetLatestSbomForBranch(branchId, CancellationToken.None);

        Assert.Equal(sbom.Id, result!.Id);
    }
}

