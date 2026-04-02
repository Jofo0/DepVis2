using DepVis.Core.Context;
using DepVis.Core.Repositories;
using DepVis.Core.Services.Interfaces;
using DepVis.Shared.Messages;
using DepVis.Shared.Model.Enums;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace DepVis.Core.Consumers;

public class IngestBranchHistoryMessageConsumer(
    ILogger<IngestBranchHistoryMessageConsumer> logger,
    ISbomProcessor sbomProcessor,
    ProjectBranchRepository projectBranchRepository,
    DepVisDbContext dbContext
) : IConsumer<IngestBranchHistoryMessage>
{
    public async Task Consume(ConsumeContext<IngestBranchHistoryMessage> context)
    {
        var message = context.Message;
        logger.LogDebug(
            "Received IngestBranchHistoryMessage for BranchHistory {branchHistoryId}",
            message.BranchHistoryId
        );

        var sbom = await dbContext.Sboms.FirstOrDefaultAsync(
            s => s.BranchHistoryId == message.BranchHistoryId,
            context.CancellationToken
        );

        var history = await projectBranchRepository.GetBranchHistoryAsync(
            message.BranchHistoryId,
            context.CancellationToken
        );

        if (sbom == null || history == null)
        {
            logger.LogWarning(
                "No SBOM found for BranchHistory {branchHistoryId}",
                message.BranchHistoryId
            );
            return;
        }

        logger.LogDebug("Publishing IngestProcessingMessage for Sbom {sbomId}", sbom.Id);

        try
        {
            await sbomProcessor.ProcessAsync(sbom, false, context.CancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error processing SBOM for BranchHistory {branchHistoryId}",
                message.BranchHistoryId
            );
            history.ProcessStatus = ProcessStatus.Failed;
        }

        history.ProcessState = HistoryProcessing.Ingesting;
        history.ProcessStatus = ProcessStatus.Success;
        await projectBranchRepository.Update(history, context.CancellationToken);

        logger.LogDebug(
            "Ingestion completed for BranchHistory {branchHistoryId}",
            message.BranchHistoryId
        );
    }
}
