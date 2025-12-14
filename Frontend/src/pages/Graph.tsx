import BranchSelector from "@/components/BranchSelector";
import SimpleGraph, { type GraphNames } from "@/components/graph/SimpleGraph";
import ParentsSelector from "@/components/graph/GraphMisc/ParentsSelector";
import Separator from "@/components/Separator";
import SeveritySelector from "@/components/graph/GraphMisc/SeveritySelector";
import { colors } from "@/theme/colors";
import type { Severity } from "@/types/packages";
import { useBranch } from "@/utils/hooks/BranchProvider";
import { useState } from "react";
import LegendItem from "@/components/graph/GraphMisc/Legend/LegendItem";
import NamesSelector from "@/components/graph/GraphMisc/NamesSelector";
import { CardDescription } from "@/components/ui/card";

const Graph = () => {
  const { branch } = useBranch();
  const [selectedSeverity, setSelectedSeverity] = useState<
    Severity | undefined
  >();
  const [showNames, setShowNames] = useState<GraphNames>("severity");
  const [showParents, setShowParents] = useState(true);

  return (
    <div className="h-[calc(100vh-5rem)] max-h-[calc(100vh-5rem)] overflow-hidden">
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
      <div className="flex flex-col gap-4 border-2 rounded-2xl bg-white p-4 absolute bottom-10 z-1000">
        <CardDescription>Legend</CardDescription>
        <div className="flex flex-row gap-4 items-center">
          <LegendItem title="Project Root" color={colors.deepPurple} />
          <LegendItem title="Critical Severity" color={colors.darkRed} />
          <LegendItem title="High Severity" color={colors.red} />
          <LegendItem title="Medium Severity" color={colors.orange} />
          <LegendItem title="Low Severity" color={colors.yellow} />
        </div>
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
