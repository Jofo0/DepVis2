using System.Web;
using DepVis.Core.Dtos;
using DepVis.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace DepVis.Core.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GitController(GitService gitService) : ControllerBase
{
    [HttpGet("{gitHubUrl}")]
    public ActionResult<GitInformationDto> RetrieveInformationAboutGitRepo(string gitHubUrl)
    {
        var url = HttpUtility.UrlDecode(gitHubUrl);
        return Ok(gitService.RetrieveInformationAboutGitRepo(url));
    }
}
