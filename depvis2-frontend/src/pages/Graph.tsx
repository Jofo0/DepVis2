import SimpleGraph from "@/components/graph/SimpleGraph";
import { useGetProjectBranchesQuery } from "@/store/api/projectsApi";
import type { ProjectBranchDto } from "@/types/projects";
import { useMemo, useEffect, useState } from "react";
import { useParams } from "react-router-dom";

const Graph = () => {
  const { id } = useParams<{ id: string }>();

  const { data: branches } = useGetProjectBranchesQuery(id!);
  const [selectedBranch, setSelectedBranch] = useState<ProjectBranchDto>();

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
    <div> {selectedBranch && <SimpleGraph branch={selectedBranch} />}</div>
  );
};

export default Graph;
