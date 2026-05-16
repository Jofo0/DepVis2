using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using DepVis.SbomProcessing.Models;
using DepVis.SbomProcessing.Options;
using DepVis.Shared.Messages;
using DepVis.Shared.Model;
using DepVis.Shared.Model.Enums;
using DepVis.Shared.Services;
using LibGit2Sharp;
using MassTransit;
using Microsoft.Extensions.Options;

namespace DepVis.SbomProcessing.Consumers;

public class BranchHistoryProcessingMessageConsumer(
    ILogger<BranchHistoryProcessingMessageConsumer> _logger,
    MinioStorageService _minioStorageService,
    IPublishEndpoint _publishEndpoint,
    ProcessingService _processingService,
    IOptions<ProcessingOptions> _processingOptions
) : IConsumer<BranchHistoryProcessingMessage>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    public async Task Consume(ConsumeContext<BranchHistoryProcessingMessage> context)
    {
        var githubLink = context.Message.GitHubLink;
        var location = context.Message.Location;
        var workerCount = Math.Max(1, _processingOptions.Value.WorkerCount);

        await _publishEndpoint.Publish(
            new UpdateBranchHistoryProcessingMessage
            {
                ProjectBranchId = context.Message.ProjectBranchId,
                ProcessStep = ProcessStep.SbomCreation,
                ProcessStatus = ProcessStatus.Pending
            }
        );

        var rootTempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var sourceRepoDir = Path.Combine(rootTempDir, "source");
        Directory.CreateDirectory(sourceRepoDir);

        try
        {
            _logger.LogDebug("Cloning repository {githubLink}", githubLink);

            var cloneOptions = new CloneOptions { Checkout = true };
            Repository.Clone(githubLink, sourceRepoDir, cloneOptions);

            _logger.LogDebug("Repository cloned successfully");

            List<CommitDescriptor> commitsToProcess;
            using (var repo = new Repository(sourceRepoDir))
            {
                var branchName = location;
                var branch = repo.Branches.FirstOrDefault(x => x.FriendlyName.EndsWith(branchName));

                if (branch == null)
                {
                    _logger.LogError(
                        "Branch {branchName} not found in the repository.",
                        branchName
                    );

                    await _publishEndpoint.Publish(
                        new UpdateProcessingMessage
                        {
                            ProjectBranchId = context.Message.ProjectBranchId,
                            ProcessStatus = ProcessStatus.Failed
                        }
                    );

                    return;
                }

                commitsToProcess =
                [
                    .. repo
                        .Commits.QueryBy(
                            new CommitFilter
                            {
                                IncludeReachableFrom = branch,
                                SortBy = CommitSortStrategies.Time
                            }
                        )
                        .Reverse()
                        .Select(c => new CommitDescriptor
                        {
                            Sha = c.Sha,
                            MessageShort = c.MessageShort,
                            CommitDate = c.Author.When.LocalDateTime
                        })
                ];
            }

            if (commitsToProcess.Count == 0)
            {
                _logger.LogInformation("No commits found to process.");

                await _publishEndpoint.Publish(
                    new UpdateBranchHistoryProcessingMessage
                    {
                        ProjectBranchId = context.Message.ProjectBranchId,
                        ProcessStep = ProcessStep.SbomCreation,
                        ProcessStatus = ProcessStatus.Success,
                        Commits = []
                    }
                );

                return;
            }

            var progress = new ProgressState { TotalCommits = commitsToProcess.Count };

            var actualWorkerCount = Math.Min(workerCount, progress.TotalCommits);

            _logger.LogInformation(
                "Preparing {workerCount} workers for {commitCount} commits",
                actualWorkerCount,
                progress.TotalCommits
            );

            var workerFolders = new List<string>(actualWorkerCount);
            for (var i = 0; i < actualWorkerCount; i++)
            {
                var workerDir = Path.Combine(rootTempDir, $"worker-{i}");
                CopyDirectory(sourceRepoDir, workerDir);
                workerFolders.Add(workerDir);
            }

            var commitChunks = SplitIntoChunks(commitsToProcess, actualWorkerCount);
            var processedResults = new ConcurrentBag<ProcessedCommitResult>();

            var workerTasks = commitChunks.Select((chunk, workerIndex) =>
                ProcessChunkAsync(
                    workerIndex,
                    workerFolders[workerIndex],
                    chunk,
                    context.Message.ProjectBranchId,
                    progress,
                    processedResults,
                    context.CancellationToken
                )
            );

            await Task.WhenAll(workerTasks);

            var globallyDedupedResults = DeduplicateAcrossWorkers(processedResults);

            await _publishEndpoint.Publish(
                new UpdateBranchHistoryProcessingMessage
                {
                    ProjectBranchId = context.Message.ProjectBranchId,
                    ProcessStep = ProcessStep.SbomCreation,
                    ProcessStatus = ProcessStatus.Success,
                    Commits = globallyDedupedResults
                }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An error occurred during the processing of {githubLink}. Message [{errorMessage}]",
                githubLink,
                ex.Message
            );

            await _publishEndpoint.Publish(
                new UpdateProcessingMessage
                {
                    ProjectBranchId = context.Message.ProjectBranchId,
                    ProcessStatus = ProcessStatus.Failed
                }
            );
        }
        finally
        {
            if (Directory.Exists(rootTempDir))
                try
                {
                    Directory.Delete(rootTempDir, true);
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(
                        ex,
                        "Failed to delete temp directory {tempDir}",
                        rootTempDir
                    );
                }
        }
    }

    private async Task ProcessChunkAsync(
        int workerIndex,
        string workerRepoDir,
        IReadOnlyList<CommitDescriptor> commits,
        Guid projectBranchId,
        ProgressState progress,
        ConcurrentBag<ProcessedCommitResult> resultBag,
        CancellationToken cancellationToken
    )
    {
        if (commits.Count == 0)
        {
            _logger.LogInformation("Worker {workerIndex} received no commits.", workerIndex);
            return;
        }

        string? lastContentHash = null;

        using var repo = new Repository(workerRepoDir);

        foreach (var commit in commits)
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                _logger.LogInformation(
                    "Worker {workerIndex} checking out commit {sha} - {message} ({date})",
                    workerIndex,
                    commit.Sha,
                    commit.MessageShort,
                    commit.CommitDate
                );

                var commitRef = repo.Lookup<Commit>(commit.Sha);
                if (commitRef == null)
                {
                    _logger.LogWarning(
                        "Worker {workerIndex} could not find commit {sha} in repo copy",
                        workerIndex,
                        commit.Sha
                    );

                    var missingProcessed = Interlocked.Increment(ref progress.CommitsProcessed);
                    await PublishProgressIfNeeded(
                        projectBranchId,
                        progress.TotalCommits,
                        missingProcessed,
                        progress,
                        cancellationToken
                    );
                    continue;
                }

                Commands.Checkout(
                    repo,
                    commitRef,
                    new CheckoutOptions { CheckoutModifiers = CheckoutModifiers.Force }
                );

                var filename = $"{Guid.NewGuid()}.cdx.sbom.json";
                var outputFile = Path.Combine(workerRepoDir, filename);

                _logger.LogInformation(
                    "Worker {workerIndex} processing commit {sha}",
                    workerIndex,
                    commit.Sha
                );

                var runnerKey = $"branch-history-{workerIndex}";
                var trivyLock = TrivyLockProvider.GetLock(runnerKey);

                await trivyLock.WaitAsync(cancellationToken);
                try
                {
                    await _processingService.RunTrivy(
                        workerRepoDir,
                        outputFile,
                        runnerKey,
                        cancellationToken
                    );
                }
                finally
                {
                    trivyLock.Release();
                }

                await using var stream = File.OpenRead(outputFile);

                var json =
                    await JsonSerializer.DeserializeAsync<CycloneDxBom>(
                        stream,
                        JsonOptions,
                        cancellationToken
                    ) ?? new CycloneDxBom { Components = [], Vulnerabilities = [] };

                var contentHash = ComputeContentHash(json);

                if (contentHash == lastContentHash)
                {
                    _logger.LogInformation(
                        "Worker {workerIndex}: no dependency changes for commit {sha}, skipping upload",
                        workerIndex,
                        commit.Sha
                    );

                    var skippedProcessed = Interlocked.Increment(ref progress.CommitsProcessed);
                    await PublishProgressIfNeeded(
                        projectBranchId,
                        progress.TotalCommits,
                        skippedProcessed,
                        progress,
                        cancellationToken
                    );
                    continue;
                }

                lastContentHash = contentHash;

                _logger.LogDebug("Worker {workerIndex} uploading SBOM file to MinIO", workerIndex);

                await _minioStorageService.UploadAsync(outputFile, filename, cancellationToken);

                resultBag.Add(
                    new ProcessedCommitResult
                    {
                        CommitInfo = new CommitProcessingInfo
                        {
                            FileName = filename,
                            CommitMessage = commit.MessageShort,
                            CommitSha = commit.Sha,
                            CommitDate = commit.CommitDate
                        },
                        PackageCount = json.Components?.Count ?? 0,
                        VulnerabilityCount = json.Vulnerabilities?.Count ?? 0,
                        ContentHash = contentHash
                    }
                );

                var processed = Interlocked.Increment(ref progress.CommitsProcessed);
                await PublishProgressIfNeeded(
                    projectBranchId,
                    progress.TotalCommits,
                    processed,
                    progress,
                    cancellationToken
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Worker {workerIndex}: error while processing commit {sha}. Message: [{errorMessage}]",
                    workerIndex,
                    commit.Sha,
                    ex.Message
                );

                var failedProcessed = Interlocked.Increment(ref progress.CommitsProcessed);
                await PublishProgressIfNeeded(
                    projectBranchId,
                    progress.TotalCommits,
                    failedProcessed,
                    progress,
                    cancellationToken
                );
            }
    }

    private async Task PublishProgressIfNeeded(
        Guid projectBranchId,
        int totalCommits,
        int commitsProcessed,
        ProgressState progress,
        CancellationToken cancellationToken
    )
    {
        if (commitsProcessed % 50 != 0 && commitsProcessed != totalCommits) return;

        var elapsedSeconds = (DateTime.UtcNow.Ticks - progress.StartedAtTicks) / TimeSpan.TicksPerSecond;
        var estimatedSecondsRemaining = commitsProcessed > 0
            ? (int)((double)elapsedSeconds / commitsProcessed * (totalCommits - commitsProcessed))
            : 0;

        _logger.LogInformation(
            "Branch history progress for {projectBranchId}: {commitsProcessed}/{totalCommits} (~{eta}s remaining)",
            projectBranchId,
            commitsProcessed,
            totalCommits,
            estimatedSecondsRemaining
        );

        await _publishEndpoint.Publish(
            new BranchProcessingCountMessage
            {
                ProjectBranchId = projectBranchId,
                TotalCommits = totalCommits,
                ProcessedCommits = commitsProcessed,
                EstimatedSecondsRemaining = estimatedSecondsRemaining
            },
            cancellationToken
        );
    }

    private static List<List<CommitDescriptor>> SplitIntoChunks(
        List<CommitDescriptor> commits,
        int workerCount
    )
    {
        var result = new List<List<CommitDescriptor>>(workerCount);

        var baseSize = commits.Count / workerCount;
        var remainder = commits.Count % workerCount;
        var offset = 0;

        for (var i = 0; i < workerCount; i++)
        {
            var size = baseSize + (i < remainder ? 1 : 0);
            result.Add(commits.Skip(offset).Take(size).ToList());
            offset += size;
        }

        return result;
    }

    private List<CommitProcessingInfo> DeduplicateAcrossWorkers(
        IEnumerable<ProcessedCommitResult> results
    )
    {
        var ordered = results
            .OrderBy(x => x.CommitInfo.CommitDate)
            .ThenBy(x => x.CommitInfo.CommitSha);

        var deduped = new List<CommitProcessingInfo>();
        ProcessedCommitResult? lastKept = null;

        foreach (var current in ordered)
        {
            if (lastKept != null && lastKept.ContentHash == current.ContentHash)
            {
                _logger.LogInformation(
                    "Globally deduplicating commit {sha}; content hash matches previous kept commit",
                    current.CommitInfo.CommitSha
                );
                continue;
            }

            deduped.Add(current.CommitInfo);
            lastKept = current;
        }

        return deduped;
    }

    private static string ComputeContentHash(CycloneDxBom bom)
    {
        var sb = new StringBuilder();

        if (bom.Components is { Count: > 0 })
            foreach (var c in bom.Components.OrderBy(c => c.Purl, StringComparer.Ordinal))
                sb.Append(c.Purl).Append('\n');

        sb.Append("||");

        if (bom.Vulnerabilities is { Count: > 0 })
            foreach (var v in bom.Vulnerabilities.OrderBy(v => v.Id, StringComparer.Ordinal))
                sb.Append(v.Id).Append('\n');

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(sb.ToString()));
        return Convert.ToHexString(hash);
    }

    private static void CopyDirectory(string sourceDir, string destinationDir)
    {
        var source = new DirectoryInfo(sourceDir);

        if (!source.Exists) throw new DirectoryNotFoundException($"Source directory not found: {sourceDir}");

        Directory.CreateDirectory(destinationDir);

        foreach (var file in source.GetFiles())
        {
            var targetFilePath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(targetFilePath, true);
        }

        foreach (var subDir in source.GetDirectories())
        {
            var newDestinationDir = Path.Combine(destinationDir, subDir.Name);
            CopyDirectory(subDir.FullName, newDestinationDir);
        }
    }
}