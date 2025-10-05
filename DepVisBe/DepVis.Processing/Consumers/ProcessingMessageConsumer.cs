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
    private static readonly SemaphoreSlim _trivyLock = new(1, 1);

    public async Task Consume(ConsumeContext<ProcessingMessage> context)
    {
        var githubLink = context.Message.GitHubLink;
        var branch = context.Message.Branch;

        await _publishEndpoint.Publish(
            new UpdateProcessingMessage
            {
                ProjectBranchId = context.Message.ProjectBranchId,
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
            var cloneOptions = new CloneOptions { BranchName = branch, Checkout = true };
            Repository.Clone(githubLink, tempDir, cloneOptions);
            _logger.LogDebug("Repository cloned successfully");

            await _trivyLock.WaitAsync(context.CancellationToken);
            try
            {
                await RunTrivy(tempDir, outputFile);
            }
            finally
            {
                _trivyLock.Release();
            }

            _logger.LogDebug("Uploading the created SBOM file to minIO storage");
            await _minioStorageService.UploadAsync(outputFile, filename);
            _logger.LogDebug("SBOM uploaded succesfully");

            await _publishEndpoint.Publish(
                new UpdateProcessingMessage
                {
                    ProjectBranchId = context.Message.ProjectBranchId,
                    ProcessStatus = Shared.Model.Enums.ProcessStatus.Success,
                    FileName = filename,
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
                new UpdateProcessingMessage
                {
                    ProjectBranchId = context.Message.ProjectBranchId,
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
            Arguments =
                $"fs --format cyclonedx --output {output} --include-dev-deps --scanners vuln .",
            WorkingDirectory = directory,
            UseShellExecute = false,
        };

        _logger.LogDebug("Running Trivy on the cloned repository");
        await RunProcessAsync(trivy);
        _logger.LogDebug("Trivy ran succesfully and the SBOM has been created");
    }

    private async Task RunSyft(string directory, string output)
    {
        var trivy = new ProcessStartInfo
        {
            FileName = "syft",
            Arguments = $". -o cyclonedx-json={output}",
            WorkingDirectory = directory,
            UseShellExecute = false,
        };

        _logger.LogDebug("Running syft on the cloned repository");
        await RunProcessAsync(trivy);
        _logger.LogDebug("syft ran succesfully and the SBOM has been created");
    }

    private async Task RunProcessAsync(ProcessStartInfo psi)
    {
        using var process = new Process { StartInfo = psi };
        process.Start();

        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
            throw new Exception(
                $"Process '{psi.FileName}' failed with exit code {process.ExitCode}"
            );
    }
}
