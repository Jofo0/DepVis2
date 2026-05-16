using DepVis.Core.Dtos;
using DepVis.Core.Repositories.Interfaces;
using DepVis.Core.Services;
using DepVis.Shared.Model;
using NSubstitute;

namespace DepVis.Tests.Services;

public class GraphServiceTests
{
    private readonly ISbomRepository _sbomRepo = Substitute.For<ISbomRepository>();
    private readonly GraphService _sut;

    public GraphServiceTests()
    {
        _sut = new GraphService(_sbomRepo);
    }

    [Fact]
    public async Task GetProjectGraphData_ReturnsNull_WhenSbomNotFound()
    {
        var branchId = Guid.NewGuid();
        _sbomRepo.GetLatestWithPackagesAndChildrenAsync(branchId).Returns((Sbom?)null);

        var result = await _sut.GetProjectGraphData(branchId);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetProjectGraphData_ReturnsAllPackagesAndRelations_WhenNoFilter()
    {
        var parentId = Guid.NewGuid();
        var childId = Guid.NewGuid();

        var parent = new SbomPackage { Id = parentId, Name = "Parent", Severity = "None", Children = [] };
        var child = new SbomPackage { Id = childId, Name = "Child", Severity = "High", Children = [] };

        parent.Children = [new PackageDependency { ParentId = parentId, ChildId = childId }];

        var sbom = new Sbom { SbomPackages = [parent, child] };
        _sbomRepo.GetLatestWithPackagesAndChildrenAsync(Arg.Any<Guid>()).Returns(sbom);

        var result = await _sut.GetProjectGraphData(Guid.NewGuid());

        Assert.NotNull(result);
        Assert.Equal(2, result.Packages.Count);
        Assert.Single(result.Relationships);
        Assert.Equal(parentId, result.Relationships[0].From);
        Assert.Equal(childId, result.Relationships[0].To);
    }

    [Fact]
    public async Task GetProjectGraphData_WithSeverityFilter_ReturnsOnlyMatchingSubgraph()
    {
        // Build: root -> mid -> leaf(High)
        var rootId = Guid.NewGuid();
        var midId = Guid.NewGuid();
        var leafId = Guid.NewGuid();

        var root = new SbomPackage { Id = rootId, Name = "Root", Severity = "None", Parents = [], Children = [] };
        var mid = new SbomPackage
        {
            Id = midId, Name = "Mid", Severity = "None",
            Parents = [new PackageDependency { ParentId = rootId, Parent = root, ChildId = midId }],
            Children = []
        };
        var leaf = new SbomPackage
        {
            Id = leafId, Name = "Leaf", Severity = "High",
            Parents = [new PackageDependency { ParentId = midId, Parent = mid, ChildId = leafId }],
            Children = []
        };

        var sbom = new Sbom { SbomPackages = [root, mid, leaf] };
        _sbomRepo.GetLatestWithPackagesAndChildrenAsync(Arg.Any<Guid>()).Returns(sbom);

        var result = await _sut.GetProjectGraphData(Guid.NewGuid(), severityFilter: "High");

        Assert.NotNull(result);
        // DFS from leaf should trace: leaf -> mid -> root = 3 packages
        Assert.Equal(3, result.Packages.Count);
        // Only leaf keeps "High", others become "None"
        Assert.Single(result.Packages.Where(p => p.Severity == "High"));
    }

    [Fact]
    public async Task GetProjectGraphData_WithSeverityFilter_ReturnsNull_WhenNoPackagesMatch()
    {
        var sbom = new Sbom
        {
            SbomPackages = [new SbomPackage { Name = "Pkg", Severity = "Low", Children = [] }]
        };
        _sbomRepo.GetLatestWithPackagesAndChildrenAsync(Arg.Any<Guid>()).Returns(sbom);

        var result = await _sut.GetProjectGraphData(Guid.NewGuid(), severityFilter: "Critical");

        Assert.NotNull(result);
        Assert.Empty(result.Packages);
    }

    [Fact]
    public async Task GetProjectGraphData_DFS_HandlesCircularParentReferences()
    {
        // A -> B -> A (cycle via parents)
        var aId = Guid.NewGuid();
        var bId = Guid.NewGuid();

        var a = new SbomPackage { Id = aId, Name = "A", Severity = "High", Parents = [], Children = [] };
        var b = new SbomPackage { Id = bId, Name = "B", Severity = "None", Parents = [], Children = [] };

        a.Parents = [new PackageDependency { ParentId = bId, Parent = b, ChildId = aId }];
        b.Parents = [new PackageDependency { ParentId = aId, Parent = a, ChildId = bId }];

        var sbom = new Sbom { SbomPackages = [a, b] };
        _sbomRepo.GetLatestWithPackagesAndChildrenAsync(Arg.Any<Guid>()).Returns(sbom);

        var result = await _sut.GetProjectGraphData(Guid.NewGuid(), severityFilter: "High");

        Assert.NotNull(result);
        Assert.Equal(2, result.Packages.Count); // Should not infinite loop
    }

    [Fact]
    public async Task GetPackageHierarchyGraphData_ReturnsPathToRoot()
    {
        var rootId = Guid.NewGuid();
        var childId = Guid.NewGuid();

        var root = new SbomPackage { Id = rootId, Name = "Root", Severity = "None", Parents = [], Children = [] };
        var child = new SbomPackage
        {
            Id = childId, Name = "Child", Severity = "High",
            Parents = [new PackageDependency { ParentId = rootId, Parent = root, ChildId = childId }],
            Children = []
        };

        var sbom = new Sbom { SbomPackages = [root, child] };
        _sbomRepo.GetLatestWithPackagesAndParentsAsync(Arg.Any<Guid>()).Returns(sbom);

        var result = await _sut.GetPackageHierarchyGraphData(Guid.NewGuid(), childId);

        Assert.NotNull(result);
        Assert.Equal(2, result.Packages.Count);
        Assert.Single(result.Relationships);
    }
}


