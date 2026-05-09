using DepVis.Core.Dtos;

namespace DepVis.Core.Services.Interfaces;

public interface IGitService
{
    GitInformationDto RetrieveInformationAboutGitRepo(string gitHubUrl);
}

