using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DepVisBe.Controllers;

[ApiController]
[Route("/processing")]
public class ProcessingController : ControllerBase
{
    private readonly ILogger<ProcessingController> _logger;

    public ProcessingController(ILogger<ProcessingController> logger)
    {
        _logger = logger;
    }

    [HttpPost("{path}")]
    public async Task ProcessProject(string path)
    {

        var filename = $"{Guid.NewGuid()}.cdx.sbom.json";

        var startInfo = new ProcessStartInfo
        {
            FileName = "syft",
            Arguments = $"{path} -o cyclonedx-json={filename}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        using var process = new Process { StartInfo = startInfo };
        process.Start();

        string stdout = await process.StandardOutput.ReadToEndAsync();
        string stderr = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        Console.WriteLine(stdout);
        if (!string.IsNullOrWhiteSpace(stderr))
            Console.Error.WriteLine(stderr);
    }
}
