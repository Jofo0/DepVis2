using System.Diagnostics;
using DepVis.Shared.Messages;
using DepVis.Shared.Services;
using LibGit2Sharp;
using MassTransit;

namespace DepVis.Processing.Consumers;

public class ProcessingMessageConsumer(
    ILogger<ProcessingMessageConsumer> logger,
    MinioStorageService minioStorageService
) : IConsumer<ProcessingMessage>
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

            await RunSyft(tempDir, outputFile);

            logger.LogDebug("Uploading the created SBOM file to minIO storage");
            await minioStorageService.UploadAsync(outputFile, filename);
            logger.LogDebug("SBOM uploaded succesfully");
        }
        catch (Exception ex)
        {
            logger.LogError(
                "An error occurred during the processing of {githubLink}. Message [{errorMessage}]",
                githubLink,
                ex.Message
            );
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    private async Task RunSyft(string directory, string output)
    {
        var syft = new ProcessStartInfo
        {
            FileName = "syft",
            Arguments = $". -o cyclonedx-json={output}",
            WorkingDirectory = directory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };

        logger.LogDebug("Running Syft on the cloned repository");
        await RunProcessAsync(syft);
        logger.LogDebug("Syft ran succesfully and the SBOM has been created");
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
            throw new Exception(
                $"Process '{psi.FileName}' failed with exit code {process.ExitCode}"
            );
    }
}
