import BranchSelector from "@/components/BranchSelector";
import SimpleGraph, { type GraphNames } from "@/components/graph/SimpleGraph";
import NamesSelector from "@/components/NamesSelector";
import ParentsSelector from "@/components/ParentsSelector";
import Separator from "@/components/Separator";
import SeveritySelector from "@/components/SeveritySelector";
import type { Severity } from "@/types/packages";
import { useBranch } from "@/utils/hooks/BranchProvider";
import { useState } from "react";

const Graph = () => {
  const { branch } = useBranch();
  const [selectedSeverity, setSelectedSeverity] = useState<
    Severity | undefined
  >();
  const [showNames, setShowNames] = useState<GraphNames>("severity");
  const [showParents, setShowParents] = useState(true);

  return (
    <div>
      <div className="flex flex-row gap-4 border-2 rounded-2xl w-3/5 p-4">
        <BranchSelector />
        <Separator />

        <SeveritySelector
          selected={selectedSeverity}
          onSelect={setSelectedSeverity}
        />
        <Separator />
        <NamesSelector selected={showNames} onSelect={setShowNames} />
        {selectedSeverity && (
          <>
            <Separator />
            <ParentsSelector selected={showParents} onSelect={setShowParents} />
          </>
        )}
      </div>
      {branch && (
        <SimpleGraph
          branch={branch}
          severityFilter={selectedSeverity}
          showNames={showNames}
          showParents={showParents}
        />
      )}
    </div>
  );
};

export default Graph;
