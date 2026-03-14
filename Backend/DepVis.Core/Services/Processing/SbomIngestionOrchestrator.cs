using DepVis.Core.Context;
using DepVis.Core.Services.Interfaces;
using DepVis.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace DepVis.Core.Services.Processing;

public class SbomIngestionOrchestrator(
    DepVisDbContext db,
    ISbomProcessor processor,
    ILogger<SbomIngestionOrchestrator> logger
) : ISbomIngestionOrchestrator
{
    public async Task ProcessAsync(
        Guid sbomId,
        bool isHistory,
        CancellationToken cancellationToken = default
    )
    {
        var sbom = await LoadSbomAsync(sbomId, cancellationToken);

        if (sbom.ProjectBranch is null)
            return;

        if (isHistory && sbom.BranchHistory is null)
            return;

        try
        {
            MarkPending(sbom, isHistory);
            await db.SaveChangesAsync(cancellationToken);

            var result = await processor.ProcessAsync(sbom, isHistory, cancellationToken);

            sbom = await LoadSbomAsync(sbomId, cancellationToken);
            MarkSuccess(sbom, result, isHistory);
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ingestion failed.");

            MarkFailed(sbom, isHistory);
            await db.SaveChangesAsync(cancellationToken);

            throw;
        }
    }

    private async Task<Sbom> LoadSbomAsync(Guid sbomId, CancellationToken cancellationToken)
    {
        var query = db.Sboms.Include(x => x.ProjectBranch).Include(x => x.BranchHistory);

        return await query.FirstAsync(x => x.Id == sbomId, cancellationToken);
    }

    private static void MarkPending(Sbom sbom, bool isHistory)
    {
        if (isHistory)
        {
            sbom.ProjectBranch!.HistoryProcessingStep = Shared.Model.Enums.ProcessStep.SbomIngest;
            sbom.ProjectBranch.HistoryProcessinStatus = Shared.Model.Enums.ProcessStatus.Pending;
            sbom.BranchHistory!.ProcessStatus = Shared.Model.Enums.ProcessStatus.Pending;
            return;
        }

        sbom.ProjectBranch!.ProcessStep = Shared.Model.Enums.ProcessStep.SbomIngest;
        sbom.ProjectBranch.ProcessStatus = Shared.Model.Enums.ProcessStatus.Pending;
    }

    private static void MarkSuccess(Sbom sbom, SbomProcessingResult result, bool isHistory)
    {
        var vulnerabilityCount = result
            .PackageVulnerabilities.DistinctBy(x => x.SbomPackageId)
            .Count();

        if (isHistory)
        {
            sbom.BranchHistory!.PackageCount = result.Packages.Count;
            sbom.BranchHistory.VulnerabilityCount = vulnerabilityCount;
            sbom.BranchHistory.ProcessStatus = Shared.Model.Enums.ProcessStatus.Success;

            sbom.ProjectBranch!.HistoryProcessinStatus = Shared.Model.Enums.ProcessStatus.Success;
            sbom.ProjectBranch.HistoryProcessingStep = Shared.Model.Enums.ProcessStep.Processed;
            return;
        }

        sbom.ProjectBranch!.PackageCount = result.Packages.Count;
        sbom.ProjectBranch.VulnerabilityCount = vulnerabilityCount;
        sbom.ProjectBranch.ProcessStatus = Shared.Model.Enums.ProcessStatus.Success;
        sbom.ProjectBranch.ProcessStep = Shared.Model.Enums.ProcessStep.Processed;
    }

    private static void MarkFailed(Sbom sbom, bool isHistory)
    {
        if (isHistory)
        {
            sbom.ProjectBranch!.HistoryProcessinStatus = Shared.Model.Enums.ProcessStatus.Failed;
            sbom.BranchHistory!.ProcessStatus = Shared.Model.Enums.ProcessStatus.Failed;
            return;
        }

        sbom.ProjectBranch!.ProcessStatus = Shared.Model.Enums.ProcessStatus.Failed;
    }
}
