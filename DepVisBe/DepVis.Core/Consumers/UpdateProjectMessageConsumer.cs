using DepVis.Core.Context;
using DepVis.Shared.Messages;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace DepVis.Core.Consumers;

public class UpdateProjectMessageConsumer(
    ILogger<UpdateProjectMessageConsumer> logger,
    DepVisDbContext dbContext
) : IConsumer<UpdateProjectMessage>
{
    public async Task Consume(ConsumeContext<UpdateProjectMessage> context)
    {
        var message = context.Message;
        logger.LogDebug("Received UpdateProjectMessage for project {projectId}", message.ProjectId);

        var project = await dbContext.Projects.FirstAsync(x => x.Id == message.ProjectId);
        if (project == null)
            return;

        project.ProcessStep = message.ProcessStep;
        project.ProcessStatus = message.ProcessStatus;

        await dbContext.SaveChangesAsync();

        logger.LogDebug("Successfully updated Project {projectId}", message.ProjectId);
    }
}
