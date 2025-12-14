using DepVis.Core.Context;
using DepVis.Shared.Messages;
using DepVis.Shared.Model;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace DepVis.Core.Consumers;

public class UpdateProcessingMessageConsumer(
    ILogger<UpdateProcessingMessageConsumer> logger,
    IPublishEndpoint publishEndpoint,
    DepVisDbContext dbContext
) : IConsumer<UpdateProcessingMessage>
{
    public async Task Consume(ConsumeContext<UpdateProcessingMessage> context)
    {
        var message = context.Message;
        logger.LogDebug(
            "Received FinishedProcessingMessage for project {projectId}",
            message.ProjectBranchId
        );

        var projectBranch = await dbContext.ProjectBranches.FirstAsync(x =>
            x.Id == message.ProjectBranchId
        );
        if (projectBranch == null)
            return;

        projectBranch.ProcessStep = Shared.Model.Enums.ProcessStep.SbomCreation;
        projectBranch.ProcessStatus = message.ProcessStatus;

        Sbom? sbom = null;

        if (message.ProcessStatus == Shared.Model.Enums.ProcessStatus.Success)
        {
            sbom = new Sbom()
            {
                FileName = message.FileName,
                ProjectBranchId = projectBranch.Id,
                CommitDate = message.CommitDate,
                CommitMessage = message.CommitMessage,
                CommitSha = message.CommitSha,
            };

            projectBranch.ScanDate = sbom.CreatedAt;
            projectBranch.CommitDate = message.CommitDate;
            projectBranch.CommitMessage = message.CommitMessage;
            projectBranch.CommitSha = message.CommitSha;

            dbContext.Sboms.Add(sbom);
        }

        await dbContext.SaveChangesAsync();

        logger.LogDebug("Successfully updated Project {projectId}", message.ProjectBranchId);

        if (message.ProcessStatus == Shared.Model.Enums.ProcessStatus.Success && sbom != null)
        {
            logger.LogDebug("Publishing IngestProcessingMessage for Sbom {sbomId}", sbom.Id);
            await publishEndpoint.Publish(new IngestProcessingMessage() { SbomId = sbom.Id });
        }
    }
}
