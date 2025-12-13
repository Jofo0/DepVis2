import BranchSelector from "@/components/BranchSelector";
import SimpleGraph from "@/components/graph/SimpleGraph";
import SeveritySelector from "@/components/SeveritySelector";
import { useGetProjectBranchesQuery } from "@/store/api/projectsApi";
import type { Branch } from "@/types/branches";
import type { Severity } from "@/types/packages";
import { useMemo, useEffect, useState } from "react";
import { useParams } from "react-router-dom";

const Graph = () => {
  const { id } = useParams<{ id: string }>();

  const { data: branches } = useGetProjectBranchesQuery(id!);
  const [selectedBranch, setSelectedBranch] = useState<Branch>();
  const [selectedSeverity, setSelectedSeverity] = useState<
    Severity | undefined
  >();

  const preferredDefault = useMemo(() => {
    if (!branches || branches.length === 0) return undefined;
    return branches[0];
  }, [branches]);

  useEffect(() => {
    if (!branches || branches.length === 0) return;
    if (!selectedBranch || !branches.includes(selectedBranch)) {
      setSelectedBranch(preferredDefault ?? branches[0]);
    }
  }, [branches, preferredDefault, selectedBranch]);
  return (
    <div>
      <div className="flex flex-row gap-4">
        <BranchSelector />
        <SeveritySelector
          selected={selectedSeverity}
          onSelect={setSelectedSeverity}
        />
      </div>
      {selectedBranch && (
        <SimpleGraph
          branch={selectedBranch}
          severityFilter={selectedSeverity}
        />
      )}
    </div>
  );
};

export default Graph;
