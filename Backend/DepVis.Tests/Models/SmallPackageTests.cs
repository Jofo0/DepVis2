using DepVis.Core.Dtos;

namespace DepVis.Tests.Models;

public class SmallPackageTests
{
    [Fact]
    public void Equals_MatchesOnNameAndVersion_IgnoresEcosystem()
    {
        var a = new SmallPackage("Pkg", "1.0", "nuget");
        var b = new SmallPackage("Pkg", "1.0", "npm");

        Assert.Equal(a, b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentVersions_AreNotEqual()
    {
        var a = new SmallPackage("Pkg", "1.0", "nuget");
        var b = new SmallPackage("Pkg", "2.0", "nuget");

        Assert.NotEqual(a, b);
    }

    [Fact]
    public void Equals_DifferentNames_AreNotEqual()
    {
        var a = new SmallPackage("PkgA", "1.0", "nuget");
        var b = new SmallPackage("PkgB", "1.0", "nuget");

        Assert.NotEqual(a, b);
    }

    [Fact]
    public void HashSet_DeduplicatesCorrectly()
    {
        var set = new HashSet<SmallPackage>
        {
            new("Pkg", "1.0", "nuget"),
            new("Pkg", "1.0", "npm"),
            new("Pkg", "2.0", "nuget")
        };

        Assert.Equal(2, set.Count);
    }
}

