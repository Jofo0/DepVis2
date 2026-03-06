namespace DepVis.Core.Services.Interfaces;

public interface ISbomIngestionOrchestrator
{
    Task ProcessAsync(Guid sbomId, bool isHistory, CancellationToken cancellationToken = default);
}
