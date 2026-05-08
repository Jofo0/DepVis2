using DepVis.Shared.Messages;
using DepVis.Shared.Services;
using LibGit2Sharp;
using MassTransit;

namespace DepVis.SbomProcessing.Consumers;

public class ProcessingMessageConsumer(
    ILogger<ProcessingMessageConsumer> logger,
    MinioStorageService minioStorageService,
    IPublishEndpoint publishEndpoint,
    ProcessingService processingService
) : IConsumer<ProcessingMessage>
{
    public async Task Consume(ConsumeContext<ProcessingMessage> context)
    {
        var githubLink = context.Message.GitHubLink;
        var branch = context.Message.GitTarget;

        await publishEndpoint.Publish(
            new UpdateProcessingMessage
            {
                ProjectBranchId = branch.ProjectBranchId,
                ProcessStatus = Shared.Model.Enums.ProcessStatus.Pending,
            }
        );

        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            logger.LogInformation("Cloning repository {githubLink}", githubLink);
            var cloneOptions = new CloneOptions { Checkout = true };
            Repository.Clone(githubLink, tempDir, cloneOptions);
            logger.LogInformation("Repository cloned successfully");

            using (var repo = new Repository(tempDir))
            {
                var checkoutOptions = new CheckoutOptions()
                {
                    CheckoutModifiers = CheckoutModifiers.Force,
                };

                logger.LogInformation("Processing branch/tag {branch}", branch.Location);

                var filename = $"{Guid.NewGuid()}.cdx.sbom.json";
                var outputFile = Path.Combine(tempDir, filename);
                string commitMessage = string.Empty;
                string commitSha = string.Empty;
                DateTime commitDate = DateTime.UtcNow;

                try
                {
                    if (branch.IsTag)
                    {
                        Tag tag =
                            repo.Tags[branch.Location]
                            ?? throw new Exception(
                                $"Tag {branch.Location} not found in the repository"
                            );

                        Commands.Checkout(repo, tag.Target.Sha, checkoutOptions);
                    }
                    else
                    {
                        Commands.Checkout(repo, branch.Location, checkoutOptions);
                    }

                    logger.LogInformation("Retrieving latest commit information");

                    var commit = repo.Head.Tip;
                    commitMessage = commit.MessageShort;
                    commitDate = commit.Author.When.LocalDateTime;
                    commitSha = commit.Sha;

                    logger.LogInformation(
                        "Latest commit: {sha} - {message} ({date})",
                        commitSha,
                        commitMessage,
                        commitDate
                    );

                    var runnerKey = Constants.BranchProcessing;
                    var trivyLock = TrivyLockProvider.GetLock(runnerKey);

                    await trivyLock.WaitAsync(context.CancellationToken);
                    try
                    {
                        await processingService.RunTrivy(
                            tempDir,
                            outputFile,
                            runnerKey,
                            context.CancellationToken
                        );
                    }
                    finally
                    {
                        trivyLock.Release();
                    }

                    logger.LogDebug("Uploading the created SBOM file to minIO storage");
                    await minioStorageService.UploadAsync(outputFile, filename);
                    logger.LogDebug("SBOM uploaded succesfully");

                    await publishEndpoint.Publish(
                        new UpdateProcessingMessage
                        {
                            ProjectBranchId = branch.ProjectBranchId,
                            ProcessStatus = Shared.Model.Enums.ProcessStatus.Success,
                            FileName = filename,
                            CommitDate = commitDate,
                            CommitMessage = commitMessage,
                            CommitSha = commitSha,
                        }
                    );
                }
                catch (Exception ex)
                {
                    logger.LogError(
                        "An error occurred during the processing of {githubLink}. Message [{errorMessage}]",
                        githubLink,
                        ex.Message
                    );

                    await publishEndpoint.Publish(
                        new UpdateProcessingMessage
                        {
                            ProjectBranchId = branch.ProjectBranchId,
                            ProcessStatus = Shared.Model.Enums.ProcessStatus.Failed,
                        }
                    );
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(
                "An error occurred during the processing of {githubLink}. Message [{errorMessage}]",
                githubLink,
                ex.Message
            );

            await publishEndpoint.Publish(
                new UpdateProcessingMessage
                {
                    ProjectBranchId = branch.ProjectBranchId,
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
                    logger.LogInformation("Failed to delete temp directory {tempDir}", tempDir);
                }
            }
        }
    }
}
