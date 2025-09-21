using DepVis.Core.Context;
using DepVis.Shared.Messages;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace DepVis.Core.Consumers;

public class UpdateProcessingMessageConsumer(
    ILogger<UpdateProcessingMessageConsumer> logger,
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

        if (message.ProcessStatus == Shared.Model.Enums.ProcessStatus.Success)
        {
            project.Sboms.Add(
                new Shared.Model.Sbom()
                {
                    FileName = message.FileName,
                    Branch = message.Branch,
                    ProjectId = project.Id,
                }
            );
        }

        await dbContext.SaveChangesAsync();

        logger.LogDebug("Successfully updated Project {projectId}", message.ProjectId);
    }
}
