using DepVis.Shared.Messages;
using LibGit2Sharp;
using MassTransit;
using System.Diagnostics;

namespace DepVis.Processing.Consumers;

public class ProcessingMessageConsumer(ILogger<ProcessingMessageConsumer> logger) : IConsumer<ProcessingMessage>
{

    public async Task Consume(ConsumeContext<ProcessingMessage> context)
    {
        var githubLink = context.Message.GitHubLink;

        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        var filename = $"{Guid.NewGuid()}.cdx.sbom.json";
        var outputFile = Path.Combine(tempDir, filename);

        try
        {
            logger.LogDebug("Cloning repository {githubLink}", githubLink);
            Repository.Clone(githubLink, tempDir);
            logger.LogDebug("Repository cloned successfully");

            var syft = new ProcessStartInfo
            {
                FileName = "syft",
                Arguments = $". -o cyclonedx-json={outputFile}",
                WorkingDirectory = tempDir,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };

            logger.LogDebug("Running Syft on the cloned repository");
            await RunProcessAsync(syft);
            logger.LogDebug("Syft ran succesfully");

            logger.LogDebug("Uploading the created SBOM file to minIO storage");
            var minio = new MinioStorageService();
            await minio.UploadAsync(outputFile, filename);
            logger.LogDebug("SBOM uploaded succesfully");
        }
        catch (Exception ex)
        {
            logger.LogError("An error occurred during the processing of {githubLink}. Message [{errorMessage}]", githubLink, ex.Message);
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    private async Task RunProcessAsync(ProcessStartInfo psi)
    {
        using var process = new Process { StartInfo = psi };
        process.Start();

        string stdout = await process.StandardOutput.ReadToEndAsync();
        string stderr = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        logger.LogDebug("Stdout for the ran process [{stdout}]", stdout);
        if (!string.IsNullOrWhiteSpace(stderr))
            logger.LogError("Stderr for the ran process [{stderr}]", stderr);


        if (process.ExitCode != 0)
            throw new Exception($"Process '{psi.FileName}' failed with exit code {process.ExitCode}");
    }

}
