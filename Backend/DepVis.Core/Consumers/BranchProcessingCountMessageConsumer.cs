using DepVis.Core.Dtos;
using DepVis.Shared.Messages;
using MassTransit;
using Microsoft.Extensions.Caching.Memory;

namespace DepVis.Core.Consumers;

public class BranchProcessingCountMessageConsumer(
    ILogger<BranchProcessingCountMessageConsumer> logger,
    IMemoryCache cache
) : IConsumer<BranchProcessingCountMessage>
{
    public Task Consume(ConsumeContext<BranchProcessingCountMessage> context)
    {
        var message = context.Message;
        logger.LogDebug(
            "Received BranchProcessingCountMessage for ProjectBranch {ProjectBranchId}",
            message.ProjectBranchId
        );

        var progress = new BranchProgressDto
        {
            ProcessedCommits = message.ProcessedCommits,
            TotalCommits = message.TotalCommits,
            EstimatedSecondsRemaining = message.EstimatedSecondsRemaining
        };

        cache.Set(
            $"branch-progress:{message.ProjectBranchId}",
            progress,
            TimeSpan.FromMinutes(10)
        );

        logger.LogDebug(
            "Cached history progress for ProjectBranch {ProjectBranchId}: {Processed}/{Total}",
            message.ProjectBranchId,
            message.ProcessedCommits,
            message.TotalCommits
        );

        return Task.CompletedTask;
    }
}