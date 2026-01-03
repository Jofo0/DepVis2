import { useMemo, useRef, useState } from "react";
import ForceGraph2D, {
  type ForceGraphMethods,
  type LinkObject,
  type NodeObject,
} from "react-force-graph-2d";
import { useGetProjectGraphQuery } from "../../store/api/projectsApi";
import Measure from "react-measure";
import type { Branch } from "@/types/branches";
import { cn } from "@/lib/utils";
import type { Severity } from "@/types/packages";
import { colors } from "@/theme/colors";
import { ChartLoader } from "../chart/PieCustomChart";

type GraphNode = {
  id: string;
  name: string;
  severity?: Severity;
  val: number;
  x?: number;
  y?: number;
};

type GraphLink = {
  source: string;
  target: string;
};

export type GraphNames = "none" | "all" | "severity";

const LABEL_FONT_SIZE = 12;
const LABEL_MARGIN = 4;

type SimpleGraphProps = {
  branch: Branch;
  packageId?: string;
  lr?: boolean;
  className?: string;
  showNames?: GraphNames;
  severityFilter?: Severity;
  showParents?: boolean;
  onNodeClick?: (node: GraphNode) => void;
};

const SimpleGraph = ({
  branch,
  className,
  packageId,
  showNames = "none",
  lr,
  severityFilter,
  showParents = true,
  onNodeClick,
}: SimpleGraphProps) => {
  const fgRef = useRef<
    | ForceGraphMethods<NodeObject<GraphNode>, LinkObject<GraphNode, GraphLink>>
    | undefined
  >(undefined);

  const { data, isFetching } = useGetProjectGraphQuery({
    id: branch.id,
    packageId,
    severityFilter,
    showParents,
  });
  const [size, setSize] = useState({ w: 0, h: 0 });

  const graphData = useMemo(() => {
    if (!data) return { nodes: [] as GraphNode[], links: [] as GraphLink[] };

    // Map packages to nodes
    const nodes: GraphNode[] = data.packages.map((p) => ({
      id: p.id,
      severity: p.severity,
      name: p.name,
      val: 1,
    }));

    // Map relationships to links
    const links: GraphLink[] = data.relationships.map((r) => ({
      source: r.to,
      target: r.from,
    }));

    // Compute degree to scale node size
    const deg = new Map<string, number>();
    links.forEach((l) => {
      deg.set(l.source, (deg.get(l.source) ?? 0) + 1);
      deg.set(l.target, (deg.get(l.target) ?? 0) + 1);
    });
    nodes.forEach((n) =>
      n.name === "ProjectRoot"
        ? (n.val = 14)
        : (n.val = Math.max(1, deg.get(n.id) ?? 1))
    );

    fgRef.current?.centerAt(0, 0, 400);
    const zoomTo =
      nodes.length < 500 ? (1 / nodes.length) * 100 : (1 / nodes.length) * 500;
    fgRef.current?.zoom(zoomTo, 400);

    return { nodes, links };
  }, [data]);

  if (isFetching) return <ChartLoader />;

  return (
    <Measure
      bounds
      onResize={({ bounds }) => {
        const w = Math.round(bounds?.width ?? 0);
        const h = Math.round(bounds?.height ?? 0);
        if (Math.abs(w - size.w) >= 4 || Math.abs(h - size.h) >= 4) {
          setSize({ w, h });
        }
      }}
    >
      {({ measureRef, contentRect }) => (
        <div
          ref={measureRef}
          className={cn("max-h-full max-w-full w-full h-full", className)}
        >
          <ForceGraph2D
            ref={fgRef}
            graphData={graphData}
            nodeLabel="name"
            height={contentRect?.bounds?.height}
            width={contentRect?.bounds?.width}
            onNodeClick={onNodeClick}
            cooldownTicks={lr ? 0 : 60}
            d3AlphaMin={0.05}
            linkDirectionalArrowLength={6}
            linkDirectionalArrowRelPos={1}
            dagMode={lr ? "rl" : undefined}
            dagLevelDistance={lr ? 75 : null}
            nodeColor={(node) => {
              const n = node as GraphNode;

              if (n.name === "ProjectRoot") return colors.deepPurple;

              if (n.id === packageId || n.severity === "critical")
                return colors.darkRed;

              if (n.severity === "high") return colors.red;
              if (n.severity === "medium") return colors.orange;
              if (n.severity === "low") return colors.yellow;

              return "";
            }}
            nodeAutoColorBy="id"
            nodeCanvasObject={(node, ctx, globalScale) => {
              const graphNode = node as GraphNode;

              if (
                showNames === "none" ||
                (showNames === "severity" &&
                  (graphNode.severity === undefined ||
                    graphNode.severity === "None"))
              ) {
                return;
              }

              const label = graphNode.name ?? String(graphNode.id);
              const fontSize = LABEL_FONT_SIZE / globalScale;
              const margin = LABEL_MARGIN / globalScale;
              const nodeValue =
                typeof graphNode.val === "number" ? graphNode.val : 1;
              const radius = Math.sqrt(nodeValue || 1) * 4;

              ctx.font = `${fontSize}px Sans-Serif`;
              ctx.textAlign = "center";
              ctx.textBaseline = "top";
              const textWidth = ctx.measureText(label).width;
              const x = graphNode.x ?? 0;
              const y = graphNode.y ?? 0;

              ctx.fillStyle = "rgba(255, 255, 255, 0.8)";
              ctx.fillRect(
                x - textWidth / 2 - margin,
                y + radius + margin,
                textWidth + margin * 2,
                fontSize + margin * 2
              );
              ctx.fillStyle = "#222";
              ctx.fillText(label, x, y + radius + margin * 2);
            }}
            nodeCanvasObjectMode={() => "after"}
          />
        </div>
      )}
    </Measure>
  );
};

export default SimpleGraph;
