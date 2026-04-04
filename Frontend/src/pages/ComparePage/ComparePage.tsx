import BranchSelector from "@/components/BranchSelector";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import {
  useGetProjectBranchesQuery,
  useLazyGetBranchComparisonQuery,
} from "@/store/api/branchesApi";
import {
  type Branch,
  type BranchCommitsDto,
  type BranchComparison,
} from "@/types/branches";
import { useBranch } from "@/utils/hooks/BranchProvider";
import { Minus, Plus } from "lucide-react";
import { useEffect, useMemo, useState } from "react";
import ListPanel from "./Components/ListPanel";
import StatCard from "./Components/StatCard";
import PageHeader from "@/components/PageHeader";
import CompareCard from "./Components/CompareCard";
import { useGetProjectId } from "@/utils/hooks/useGetProjectId";

const ComparePage = () => {
  const id = useGetProjectId();
  const [triggerCompare, { data, isLoading, isFetching }] =
    useLazyGetBranchComparisonQuery();
  const { data: branchesData } = useGetProjectBranchesQuery(id!);

  const { branch, commit } = useBranch();
  const [compareBranch, setCompareBranch] = useState<Branch | null>(null);
  const [compareCommit, setCompareCommit] = useState<BranchCommitsDto | null>(
    null,
  );

  useEffect(() => {
    if (branch && compareBranch) {
      triggerCompare({
        branchId: commit?.commitId ?? branch.id,
        compareToBranchId: compareCommit?.commitId ?? compareBranch?.id ?? "",
      });
    }
  }, [branch, commit, compareBranch, compareCommit, triggerCompare]);

  useEffect(() => {
    const updatedBranch =
      branchesData?.items.find((x) => x.id === compareBranch?.id) ?? null;

    if (updatedBranch) {
      setCompareBranch(updatedBranch);

      if (compareCommit && updatedBranch) {
        const updatedCommit = updatedBranch.commits.find(
          (c) => c.commitId === compareCommit?.commitId,
        );
        if (updatedCommit) {
          setCompareCommit(updatedCommit);
        }
      }
    }
  }, [branchesData]);

  const comparison = data as BranchComparison | undefined;

  const comparedBranchName = `${compareBranch?.name ?? ""} ${compareCommit?.commitName ? "- " + compareCommit.commitName : ""}`;

  const derived = useMemo(() => {
    if (!comparison) return null;

    const packageDelta =
      comparison.comparedBranchPackageCount - comparison.mainBranchPackageCount;
    const vulnDelta =
      comparison.comparedBranchVulnerabilityCount -
      comparison.mainBranchVulnerabilityCount;

    return {
      packageDelta,
      vulnDelta,
      addedPackagesCount: comparison.addedPackages.length,
      removedPackagesCount: comparison.removedPackages.length,
      addedVulnsCount: comparison.addedVulnerabilityIds.length,
      removedVulnsCount: comparison.removedVulnerabilityIds.length,
      netPackages:
        comparison.addedPackages.length - comparison.removedPackages.length,
      netVulns:
        comparison.addedVulnerabilityIds.length -
        comparison.removedVulnerabilityIds.length,
    };
  }, [comparison]);

  return (
    <div className="flex flex-col gap-4">
      <PageHeader
        title="Compare"
        description="Compare packages and vulnerabilities between sources"
        secondaryDescription="Select two sources to see what changed."
      >
        <div>
          <div className="mb-2 text-xs font-medium text-muted-foreground">
            Compare to
          </div>
          <BranchSelector
            branch={compareBranch}
            setBranch={setCompareBranch}
            commit={compareCommit}
            setCommit={setCompareCommit}
          />
        </div>
      </PageHeader>

      {(isLoading || isFetching) && (
        <div className="grid gap-3 md:grid-cols-2 lg:grid-cols-4">
          {Array.from({ length: 4 }).map((_, i) => (
            <Card key={i}>
              <CardHeader className="pb-3">
                <Skeleton className="h-4 w-40" />
                <Skeleton className="h-3 w-56" />
              </CardHeader>
              <CardContent className="space-y-2">
                <Skeleton className="h-4 w-full" />
                <Skeleton className="h-4 w-full" />
                <Skeleton className="h-4 w-1/2" />
              </CardContent>
            </Card>
          ))}
        </div>
      )}

      {!isLoading && !isFetching && !comparison && (
        <Card>
          <CardHeader>
            <CardTitle className="text-base">No comparison yet</CardTitle>
            <CardDescription>
              Pick a main branch and a compare branch to load results.
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="rounded-lg border border-dashed p-6 text-sm text-muted-foreground">
              Nothing to display.
            </div>
          </CardContent>
        </Card>
      )}

      {!isLoading && !isFetching && comparison && derived && (
        <>
          <div className="grid gap-3 md:grid-cols-2 lg:grid-cols-4">
            <StatCard
              title="Packages"
              description="Total package count per side"
              leftLabel={`${branch?.name} ${commit?.commitName ? "- " + commit.commitName : ""}`}
              leftValue={comparison.mainBranchPackageCount}
              rightLabel={comparedBranchName}
              rightValue={comparison.comparedBranchPackageCount}
              delta={derived.packageDelta}
            />
            <StatCard
              title="Vulnerabilities"
              description="Total vulnerability count per side"
              leftLabel={`${branch?.name} ${commit?.commitName ? "- " + commit.commitName : ""}`}
              leftValue={comparison.mainBranchVulnerabilityCount}
              rightLabel={comparedBranchName}
              rightValue={comparison.comparedBranchVulnerabilityCount}
              delta={derived.vulnDelta}
            />

            <CompareCard
              addedCount={derived.addedPackagesCount}
              removedCount={derived.removedPackagesCount}
              title="Package changes"
              description="Added vs removed"
              net={derived.netPackages}
            />
            <CompareCard
              addedCount={derived.addedVulnsCount}
              removedCount={derived.removedVulnsCount}
              title="Vulnerability changes"
              description="Added vs removed"
              net={derived.netVulns}
            />
          </div>

          <div className="grid gap-3 lg:grid-cols-2">
            <ListPanel
              title={`Added packages (in ${comparedBranchName})`}
              icon={<Plus className="h-4 w-4" />}
              items={comparison.addedPackages}
              nameCounts={comparison.addedEcosystems}
              emptyLabel="No packages were added."
            />
            <ListPanel
              title={`Removed packages (in ${comparedBranchName})`}
              icon={<Minus className="h-4 w-4" />}
              items={comparison.removedPackages}
              nameCounts={comparison.removedEcosystems}
              emptyLabel="No packages were removed."
            />
            <ListPanel
              title={`Added vulnerabilities (in ${comparedBranchName})`}
              icon={<Plus className="h-4 w-4" />}
              items={comparison.addedVulnerabilityIds}
              emptyLabel="No vulnerabilities were added."
            />
            <ListPanel
              title={`Removed vulnerabilities (in ${comparedBranchName})`}
              icon={<Minus className="h-4 w-4" />}
              items={comparison.removedVulnerabilityIds}
              emptyLabel="No vulnerabilities were removed."
            />
          </div>
        </>
      )}
    </div>
  );
};

export default ComparePage;
