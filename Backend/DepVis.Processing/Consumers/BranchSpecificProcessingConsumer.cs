using System.Diagnostics;
using System.Text.Json;
using DepVis.SbomProcessing;
using DepVis.Shared.Messages;
using DepVis.Shared.Model;
using DepVis.Shared.Services;
using LibGit2Sharp;
using MassTransit;

namespace DepVis.Processing.Consumers;

public class BranchSpecificProcessingConsumer(
    ILogger<BranchSpecificProcessingConsumer> _logger,
    MinioStorageService _minioStorageService,
    IPublishEndpoint _publishEndpoint,
    ProcessingService _processingService
) : IConsumer<BranchHistoryProcessingMessage>
{
    private static readonly SemaphoreSlim _trivyLock = new(1, 1);
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
    };

    public async Task Consume(ConsumeContext<BranchHistoryProcessingMessage> context)
    {
        var githubLink = context.Message.GitHubLink;
        var maxCommits = context.Message.MaxCommits;
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
                var branch = repo.Branches.FirstOrDefault(x => x.FriendlyName.EndsWith(branchName));

                if (branch == null)
                {
                    _logger.LogError(
                        "Branch {branchName} not found in the repository.",
                        branchName
                    );
                    return;
                }

                var commits = repo
                    .Commits.QueryBy(
                        new CommitFilter
                        {
                            IncludeReachableFrom = branch,
                            SortBy = CommitSortStrategies.Time,
                        }
                    )
                    .Reverse();

                long lastVulnCount = -1;
                long lastPackageCount = -1;

                foreach (var commit in commits)
                {
                    try
                    {
                        var filename = $"{Guid.NewGuid()}.cdx.sbom.json";
                        var outputFile = Path.Combine(tempDir, filename);
                        count++;

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

                        _logger.LogInformation("Processing commit");

                        await _trivyLock.WaitAsync(context.CancellationToken);
                        try
                        {
                            await _processingService.RunTrivy(tempDir, outputFile);
                        }
                        finally
                        {
                            _trivyLock.Release();
                        }

                        using Stream stream = File.OpenRead(outputFile);

                        var json =
                            JsonSerializer.Deserialize<CycloneDxBom>(stream, JsonOptions)
                            ?? new CycloneDxBom { Components = [], Vulnerabilities = [] };

                        if (
                            json.Components?.Count == lastPackageCount
                            && json.Vulnerabilities?.Count == lastVulnCount
                        )
                        {
                            _logger.LogInformation(
                                "No changes in packages and vulnerabilities compared to the last processed commit. Skipping upload for commit {sha}",
                                commit.Sha
                            );
                            continue;
                        }
                        else
                        {
                            lastPackageCount = json.Components?.Count ?? 0;
                            lastVulnCount = json.Vulnerabilities?.Count ?? 0;
                        }

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
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            "An error occurred while processing commit {sha}. Message: [{errorMessage}]",
                            commit.Sha,
                            ex.Message
                        );
                        continue;
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
}
