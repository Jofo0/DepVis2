import BranchSelector, {
  GlobalBranchSelector,
} from "@/components/BranchSelector";
import { Card, CardTitle } from "@/components/ui/card";
import { useLazyGetBranchComparisonQuery } from "@/store/api/branchesApi";
import { Branch, type BranchCommitsDto } from "@/types/branches";
import { useBranch } from "@/utils/hooks/BranchProvider";
import { useEffect, useState } from "react";

const ComparePage = () => {
  const [triggerCompare, { data, isLoading }] =
    useLazyGetBranchComparisonQuery();

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

  return (
    <div className="gap-2 flex flex-col">
      <div className="flex flex-row">
        <GlobalBranchSelector />
        <BranchSelector
          branch={compareBranch}
          setBranch={setCompareBranch}
          commit={compareCommit}
          setCommit={setCompareCommit}
        />
      </div>
      <div className="flex flex-col w-full gap-2">
        <Card>
          <CardTitle>Comparison Statistics</CardTitle>
        </Card>
        <div className="flex flex-row w-full h-full gap-2">
          <div className="w-1/2 h-full">
            <Card className="h-full">
              <CardTitle>Added in branch1 compared with branch2</CardTitle>
              <Card>
                <CardTitle>Packages</CardTitle>
              </Card>
              <Card>
                <CardTitle>Vulnerabilities</CardTitle>
              </Card>
            </Card>
          </div>
          <div className="w-1/2 h-full">
            <Card>
              <CardTitle>Removed in branch1 compared with branch2</CardTitle>
              <Card>
                <CardTitle>Packages</CardTitle>
              </Card>
              <Card>
                <CardTitle>Vulnerabilities</CardTitle>
              </Card>
            </Card>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ComparePage;
