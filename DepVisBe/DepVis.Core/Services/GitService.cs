using DepVis.Core.Dtos;
using LibGit2Sharp;

namespace DepVis.Core.Services;

public class GitService
{
    public GitInformationDto RetrieveInformationAboutGitRepo(string gitHubUrl)
    {
        Console.WriteLine($"Listing refs from remote: {gitHubUrl}\n");

        var references = Repository.ListRemoteReferences(gitHubUrl);

        var branches = references
            .Where(r => r.CanonicalName.StartsWith("refs/heads/"))
            .Select(branch => branch.CanonicalName.Replace("refs/heads/", ""))
            .ToList();

        var tags = references
            .Where(r => r.CanonicalName.StartsWith("refs/tags/"))
            .Select(tag => tag.CanonicalName.Replace("refs/tags/", ""))
            .ToList();

        return new GitInformationDto() { Branches = branches, Tags = tags };
    }
}
