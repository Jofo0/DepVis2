using System.Diagnostics;
using DepVis.Shared.Messages;
using DepVis.Shared.Services;
using LibGit2Sharp;
using MassTransit;

namespace DepVis.Processing.Consumers;

public class BranchSpecificProcessingConsumer(
    ILogger<BranchSpecificProcessingConsumer> _logger,
    MinioStorageService _minioStorageService,
    IPublishEndpoint _publishEndpoint
) : IConsumer<BranchHistoryProcessingMessage>
{
    private static readonly SemaphoreSlim _trivyLock = new(1, 1);

    public async Task Consume(ConsumeContext<BranchHistoryProcessingMessage> context)
    {
        var githubLink = context.Message.GitHubLink;
        var location = context.Message.Location;

        await _publishEndpoint.Publish(
            new UpdateBranchHistoryProcessingMessage
            {
                ProjectBranchId = context.Message.ProjectBranchId,
                ProcessStep = Shared.Model.Enums.ProcessStep.SbomCreation,
                ProcessStatus = Shared.Model.Enums.ProcessStatus.Pending,
            }
        );

        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        List<CommitProcessingInfo> commitProcessingInfos = [];

        var count = 0;
        try
        {
            _logger.LogDebug("Cloning repository {githubLink}", githubLink);
            var cloneOptions = new CloneOptions { Checkout = true };
            Repository.Clone(githubLink, tempDir, cloneOptions);
            _logger.LogDebug("Repository cloned successfully");

            string commitMessage = string.Empty;
            string commitSha = string.Empty;
            DateTime commitDate = DateTime.Now;
            using (var repo = new Repository(tempDir))
            {
                string branchName = location;
                var branch = repo.Branches[branchName];

                if (branch == null)
                {
                    _logger.LogError(
                        "Branch {branchName} not found in the repository.",
                        branchName
                    );
                    return;
                }

                var commits = repo.Commits.QueryBy(
                    new CommitFilter
                    {
                        IncludeReachableFrom = branch,
                        SortBy = CommitSortStrategies.Topological,
                    }
                );

                long lastFileSize = -1;
                foreach (var commit in commits)
                {
                    var filename = $"{Guid.NewGuid()}.cdx.sbom.json";
                    var outputFile = Path.Combine(tempDir, filename);
                    count++;
                    if (count >= 10)
                        break;

                    _logger.LogInformation(
                        "Checking out commit {sha} - {message} ({date})",
                        commit.Sha,
                        commit.MessageShort,
                        commit.Author.When.LocalDateTime
                    );

                    var checkoutOptions = new CheckoutOptions()
                    {
                        CheckoutModifiers = CheckoutModifiers.Force,
                    };

                    Commands.Checkout(repo, commit, checkoutOptions);

                    commitMessage = commit.MessageShort;
                    commitDate = commit.Author.When.LocalDateTime;
                    commitSha = commit.Sha;

                    _logger.LogInformation(
                        "Processing commit: {sha} - {message} ({date})",
                        commitSha,
                        commitMessage,
                        commitDate
                    );

                    await _trivyLock.WaitAsync(context.CancellationToken);
                    try
                    {
                        await RunTrivy(tempDir, outputFile);
                    }
                    finally
                    {
                        _trivyLock.Release();
                    }

                    long sbomSize = new FileInfo(outputFile).Length;
                    if (lastFileSize == sbomSize)
                    {
                        _logger.LogInformation(
                            "SBOM file size is identical to the previous one. Skipping upload."
                        );
                        continue;
                    }
                    lastFileSize = sbomSize;

                    _logger.LogDebug("Uploading the created SBOM file to minIO storage");
                    await _minioStorageService.UploadAsync(outputFile, filename);
                    _logger.LogDebug("SBOM uploaded successfully");

                    commitProcessingInfos.Add(
                        new CommitProcessingInfo
                        {
                            FileName = filename,
                            CommitMessage = commitMessage,
                            CommitSha = commitSha,
                            CommitDate = commitDate,
                        }
                    );
                }
            }
            await _publishEndpoint.Publish(
                new UpdateBranchHistoryProcessingMessage
                {
                    ProjectBranchId = context.Message.ProjectBranchId,
                    ProcessStep = Shared.Model.Enums.ProcessStep.SbomCreation,
                    ProcessStatus = Shared.Model.Enums.ProcessStatus.Success,
                    Commits = commitProcessingInfos,
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
