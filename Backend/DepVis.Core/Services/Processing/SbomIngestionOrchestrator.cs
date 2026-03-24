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

            if (!isHistory)
                await UpdateProjectStatistics(sbom, result, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ingestion failed.");

            MarkFailed(sbom, isHistory);
            await db.SaveChangesAsync(cancellationToken);

            throw;
        }
    }

    private async Task UpdateProjectStatistics(
        Sbom sbom,
        SbomProcessingResult newData,
        CancellationToken cancellationToken
    )
    {
        var strategy = db.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await db.Database.BeginTransactionAsync(
                cancellationToken
            );

            var projectStats = await db.ProjectStatistics.FirstOrDefaultAsync(
                x => x.ProjectId == sbom.ProjectBranch.ProjectId,
                cancellationToken
            );

            if (projectStats == null)
            {
                logger.LogDebug(
                    "Project statistics not found for project {ProjectId}. Creating them",
                    sbom.ProjectBranch.ProjectId
                );

                db.ProjectStatistics.Add(
                    new ProjectStatistics
                    {
                        Id = Guid.NewGuid(),
                        ProjectId = sbom.ProjectBranch.ProjectId,
                        PackageCount = newData.Packages.Count,
                        VulnerabilityCount = newData.PackageVulnerabilities.Count,
                        EcoSystems = string.Join(",", newData.EcoSystems),
                    }
                );
            }
            else if (projectStats.PackageCount < newData.Packages.Count)
            {
                logger.LogDebug(
                    "Updating project statistics for project {ProjectId}.",
                    sbom.ProjectBranch.ProjectId
                );

                projectStats.PackageCount = newData.Packages.Count;
                projectStats.VulnerabilityCount = newData.PackageVulnerabilities.Count;
                projectStats.EcoSystems = string.Join(",", newData.EcoSystems);
            }

            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        });
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
        if (isHistory)
        {
            sbom.BranchHistory!.PackageCount = result.Packages.Count;
            sbom.BranchHistory.DirectVulnerabilityCount = result.DirectVulnerabilities.Count;
            sbom.BranchHistory.VulnerabilityCount = result.PackageVulnerabilities.Count;
            sbom.BranchHistory.ProcessStatus = Shared.Model.Enums.ProcessStatus.Success;

            sbom.ProjectBranch!.HistoryProcessinStatus = Shared.Model.Enums.ProcessStatus.Success;
            sbom.ProjectBranch.HistoryProcessingStep = Shared.Model.Enums.ProcessStep.Processed;
            return;
        }

        sbom.ProjectBranch!.PackageCount = result.Packages.Count;
        sbom.ProjectBranch.VulnerabilityCount = result.PackageVulnerabilities.Count;
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
