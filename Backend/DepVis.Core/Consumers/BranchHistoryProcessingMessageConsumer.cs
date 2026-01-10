using DepVis.Core.Context;
using DepVis.Shared.Messages;
using DepVis.Shared.Model;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace DepVis.Core.Consumers;

public class BranchHistoryProcessingMessageConsumer(
    ILogger<BranchHistoryProcessingMessageConsumer> logger,
    IPublishEndpoint publishEndpoint,
    DepVisDbContext dbContext
) : IConsumer<UpdateBranchHistoryProcessingMessage>
{
    public async Task Consume(ConsumeContext<UpdateBranchHistoryProcessingMessage> context)
    {
        var message = context.Message;
        logger.LogDebug(
            "Received BranchHistoryProcessingMessage for ProjectBranch {projectBranchId}",
            message.ProjectBranchId
        );

        var projectBranch = await dbContext.ProjectBranches.FirstAsync(x =>
            x.Id == message.ProjectBranchId
        );
        if (projectBranch == null)
            return;

        projectBranch.HistoryProcessingStep = Shared.Model.Enums.ProcessStep.SbomCreation;
        projectBranch.HistoryProcessinStatus = message.ProcessStatus;

        List<Sbom>? sboms = [];

        if (message.ProcessStatus == Shared.Model.Enums.ProcessStatus.Success)
        {
            foreach (var commitInfo in message.Commits)
            {
                var historyId = Guid.NewGuid();
                Sbom sbom = new()
                {
                    BranchHistoryId = historyId,
                    FileName = commitInfo.FileName,
                    ProjectBranchId = projectBranch.Id,
                    CommitDate = commitInfo.CommitDate,
                    CommitMessage = commitInfo.CommitMessage,
                    CommitSha = commitInfo.CommitSha,
                };

                BranchHistory branchHistory = new()
                {
                    Id = historyId,
                    ProjectBranchId = projectBranch.Id,
                    CommitDate = commitInfo.CommitDate,
                    CommitMessage = commitInfo.CommitMessage,
                    CommitSha = commitInfo.CommitSha,
                    PackageCount = 0,
                    VulnerabilityCount = 0,
                };
                dbContext.Sboms.Add(sbom);
                dbContext.BranchHistories.Add(branchHistory);

                sboms.Add(sbom);
            }
        }

        await dbContext.SaveChangesAsync();

        logger.LogDebug(
            "Successfully updated ProjectHistory for {projectId}",
            message.ProjectBranchId
        );

        if (message.ProcessStatus == Shared.Model.Enums.ProcessStatus.Success)
        {
            foreach (var sbom in sboms)
            {
                logger.LogDebug("Publishing IngestProcessingMessage for Sbom {sbomId}", sbom.Id);
                await publishEndpoint.Publish(
                    new IngestProcessingMessage() { SbomId = sbom.Id, IsHistory = true }
                );
            }
        }
    }
}
