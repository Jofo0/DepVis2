using System.Diagnostics;
using DepVis.Shared.Messages;
using DepVis.Shared.Services;
using LibGit2Sharp;
using MassTransit;

namespace DepVis.Processing.Consumers;

public class ProcessingMessageConsumer(
    ILogger<ProcessingMessageConsumer> _logger,
    MinioStorageService _minioStorageService,
    IPublishEndpoint _publishEndpoint
) : IConsumer<ProcessingMessage>
{
    public async Task Consume(ConsumeContext<ProcessingMessage> context)
    {
        var githubLink = context.Message.GitHubLink;
        var branch = context.Message.Branch;

        await _publishEndpoint.Publish(
            new UpdateProcessingMessage()
            {
                ProjectId = context.Message.ProjectId,
                ProcessStatus = Shared.Model.Enums.ProcessStatus.Pending,
            }
        );

        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        var filename = $"{Guid.NewGuid()}.cdx.sbom.json";
        var outputFile = Path.Combine(tempDir, filename);

        try
        {
            _logger.LogDebug("Cloning repository {githubLink}", githubLink);
            var cloneOptions = new CloneOptions() { BranchName = branch, Checkout = true };
            Repository.Clone(githubLink, tempDir);
            _logger.LogDebug("Repository cloned successfully");

            await RunTrivy(tempDir, outputFile);

            _logger.LogDebug("Uploading the created SBOM file to minIO storage");
            await _minioStorageService.UploadAsync(outputFile, filename);
            _logger.LogDebug("SBOM uploaded succesfully");

            await _publishEndpoint.Publish(
                new UpdateProcessingMessage()
                {
                    ProjectId = context.Message.ProjectId,
                    ProcessStatus = Shared.Model.Enums.ProcessStatus.Success,
                    FileName = filename,
                    Branch = context.Message.Branch,
                }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "An error occurred during the processing of {githubLink}. Message [{errorMessage}]",
                githubLink,
                ex.Message
            );

            await _publishEndpoint.Publish(
                new UpdateProcessingMessage()
                {
                    ProjectId = context.Message.ProjectId,
                    ProcessStatus = Shared.Model.Enums.ProcessStatus.Failed,
                }
            );
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                try
                {
                    Directory.Delete(tempDir, true);
                }
                catch
                {
                    _logger.LogInformation("Failed to delete temp directory {tempDir}", tempDir);
                }
            }
        }
    }

    private async Task RunTrivy(string directory, string output)
    {
        var trivy = new ProcessStartInfo
        {
            FileName = "trivy",
            Arguments = $"fs --format cyclonedx --output {output} .",
            WorkingDirectory = directory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };

        _logger.LogDebug("Running Trivy on the cloned repository");
        await RunProcessAsync(trivy);
        _logger.LogDebug("Trivy ran succesfully and the SBOM has been created");
    }

    private async Task RunProcessAsync(ProcessStartInfo psi)
    {
        using var process = new Process { StartInfo = psi };
        process.Start();

        string stdout = await process.StandardOutput.ReadToEndAsync();
        string stderr = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        _logger.LogDebug("Stdout for the ran process [{stdout}]", stdout);
        if (!string.IsNullOrWhiteSpace(stderr))
            _logger.LogError("Stderr for the ran process [{stderr}]", stderr);

        if (process.ExitCode != 0)
            throw new Exception(
                $"Process '{psi.FileName}' failed with exit code {process.ExitCode}"
            );
    }
}
