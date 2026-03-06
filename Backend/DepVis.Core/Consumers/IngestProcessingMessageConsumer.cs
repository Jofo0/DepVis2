using DepVis.Core.Services;
using DepVis.Core.Services.Interfaces;
using DepVis.Shared.Messages;
using MassTransit;

namespace DepVis.Core.Consumers;

public class IngestProcessingMessageConsumer(
    ILogger<IngestProcessingMessageConsumer> logger,
    ISbomIngestionOrchestrator orchestrator
) : IConsumer<IngestProcessingMessage>
{
    public async Task Consume(ConsumeContext<IngestProcessingMessage> context)
    {
        using var _ = logger.BeginScope(
            new Dictionary<string, object?> { ["SbomId"] = context.Message.SbomId }
        );

        logger.LogDebug("Starting ingestion.");

        await orchestrator.ProcessAsync(
            context.Message.SbomId,
            context.Message.IsHistory,
            context.CancellationToken
        );

        logger.LogDebug("Ingestion completed.");
    }
}
