import { useMemo } from "react";
import ForceGraph2D from "react-force-graph-2d";
import { useGetProjectGraphQuery } from "../../store/api/projectsApi";

type GraphNode = {
  id: string;
  name: string;
  val: number;
  x?: number;
  y?: number;
};

type GraphLink = {
  source: string;
  target: string;
};

const LABEL_FONT_SIZE = 12;
const LABEL_MARGIN = 4;

const SimpleGraph = ({
  projectId,
  branch,
}: {
  projectId: string;
  branch: string;
}) => {
  const { data, isLoading, isFetching, error } = useGetProjectGraphQuery({
    id: projectId,
    branch,
  });

  const graphData = useMemo(() => {
    if (!data) return { nodes: [] as GraphNode[], links: [] as GraphLink[] };

    // Map packages to nodes
    const nodes: GraphNode[] = data.packages.map((p) => ({
      id: String(p.id),
      name: p.name,
      val: 1,
    }));

    // Map relationships to links
    const links: GraphLink[] = data.relationships.map((r) => ({
      source: String(r.to),
      target: String(r.from),
    }));

    // Compute degree to scale node size
    const deg = new Map<string, number>();
    links.forEach((l) => {
      deg.set(l.source, (deg.get(l.source) ?? 0) + 1);
      deg.set(l.target, (deg.get(l.target) ?? 0) + 1);
    });
    nodes.forEach((n) => (n.val = Math.max(1, deg.get(n.id) ?? 1)));

    return { nodes, links };
  }, [data]);

  if (isLoading || isFetching)
    return <div style={{ padding: 12 }}>Loading graph…</div>;
  if (error) return <div style={{ padding: 12 }}>Failed to load graph.</div>;
  if (!data) return <div style={{ padding: 12 }}>No graph data.</div>;

  return (
    <div style={{ width: "100%", height: "600px" }}>
      <ForceGraph2D
        graphData={graphData}
        nodeLabel="name"
        nodeAutoColorBy="id"
        linkDirectionalArrowLength={6}
        linkDirectionalArrowRelPos={1}
        nodeCanvasObject={(node, ctx, globalScale) => {
          const graphNode = node as GraphNode;
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
  );
};

export default SimpleGraph;
