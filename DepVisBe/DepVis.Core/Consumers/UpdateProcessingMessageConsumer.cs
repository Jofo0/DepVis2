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
            message.ProjectId
        );

        var project = await dbContext.Projects.FirstAsync(x => x.Id == message.ProjectId);
        if (project == null)
            return;

        project.ProcessStep = Shared.Model.Enums.ProcessStep.SbomCreation;
        project.ProcessStatus = message.ProcessStatus;

        Sbom? sbom = null;

        if (message.ProcessStatus == Shared.Model.Enums.ProcessStatus.Success)
        {
            sbom = new Sbom()
            {
                FileName = message.FileName,
                Branch = message.Branch,
                ProjectId = project.Id,
            };

            dbContext.Sboms.Add(sbom);
        }

        await dbContext.SaveChangesAsync();

        logger.LogDebug("Successfully updated Project {projectId}", message.ProjectId);

        if (message.ProcessStatus == Shared.Model.Enums.ProcessStatus.Success && sbom != null)
        {
            logger.LogDebug("Publishing IngestProcessingMessage for Sbom {sbomId}", sbom.Id);
            await publishEndpoint.Publish(new IngestProcessingMessage() { SbomId = sbom.Id });
        }
    }
}
