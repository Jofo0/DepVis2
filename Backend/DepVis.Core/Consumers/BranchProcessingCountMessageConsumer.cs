using DepVis.Core.Context;
using DepVis.Shared.Messages;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace DepVis.Core.Consumers;

public class BranchProcessingCountMessageConsumer(
    ILogger<BranchProcessingCountMessageConsumer> logger,
    DepVisDbContext dbContext
) : IConsumer<BranchProcessingCountMessage>
{
    public async Task Consume(ConsumeContext<BranchProcessingCountMessage> context)
    {
        var message = context.Message;
        logger.LogDebug(
            "Received BranchProcessingCountMessage for ProjectBranch {ProjectBranchId}",
            message.ProjectBranchId
        );

        var projectBranch = await dbContext.ProjectBranches.FirstAsync(x =>
            x.Id == message.ProjectBranchId
        );
        if (projectBranch == null)
            return;

        projectBranch.TotalHistoryCommits = message.TotalCommits;
        projectBranch.ProcessedHistoryCommits = message.ProcessedCommits;

        await dbContext.SaveChangesAsync();

        logger.LogDebug(
            "Successfully updated history commits for ProjectBranch {ProjectBranchId}",
            message.ProjectBranchId
        );
    }
}
