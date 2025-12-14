import BranchSelector from "@/components/BranchSelector";
import SimpleGraph, { type GraphNames } from "@/components/graph/SimpleGraph";
import ParentsSelector from "@/components/graph/GraphMisc/ParentsSelector";
import Separator from "@/components/Separator";
import SeveritySelector from "@/components/graph/GraphMisc/SeveritySelector";
import type { Severity } from "@/types/packages";
import { useBranch } from "@/utils/hooks/BranchProvider";
import { useState } from "react";
import NamesSelector from "@/components/graph/GraphMisc/NamesSelector";
import Legend from "@/components/graph/GraphMisc/Legend/Legend";
import NodeInformation from "@/components/graph/GraphMisc/NodeInformation/NodeInformation";

const Graph = () => {
  const { branch } = useBranch();
  const [selectedSeverity, setSelectedSeverity] = useState<
    Severity | undefined
  >();
  const [showNames, setShowNames] = useState<GraphNames>("severity");
  const [showParents, setShowParents] = useState(true);
  const [selectedNode, setSelectedNode] = useState<string | undefined>();

  return (
    <div className="h-[calc(100vh-5rem)] max-h-[calc(100vh-5rem)] w-[calc(100vw-25rem)] max-w-[calc(100vw-25rem)] overflow-hidden">
      <div className="flex flex-row gap-4 border-2 rounded-2xl  p-4 absolute top-20 z-1000 bg-white">
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
      <Legend />
      <NodeInformation packageId={selectedNode} />
      {branch && (
        <SimpleGraph
          onNodeClick={(node) => {
            setSelectedNode(node.id);
          }}
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
