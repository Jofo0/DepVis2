using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DepVisBe.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProcessingController : ControllerBase
    {
        private readonly ILogger<ProcessingController> _logger;

        public ProcessingController(ILogger<ProcessingController> logger)
        {
            _logger = logger;
        }

        [HttpPost("id")]
        public void ProcessProject(string id)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "syft",
                Arguments = $"scan \"{sourcePath}\" -o syft-json=\"{outputFilePath}\"",
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

            return process.ExitCode;
        }
    }
}
