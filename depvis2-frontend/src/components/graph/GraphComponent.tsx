import ForceGraph2D from "react-force-graph-2d";

type GraphNode = {
  id: string;
  name: string;
  val: number;
  x?: number;
  y?: number;
};

const LABEL_FONT_SIZE = 12;
const LABEL_MARGIN = 4;

export default function SimpleGraph() {
  // Example data
  const data = {
    nodes: [
      { id: "node1", name: "Node 1", val: 1 },
      { id: "node2", name: "Node 2", val: 2 },
      { id: "node3", name: "Node 3", val: 3 },
      { id: "node4", name: "Node 4", val: 4 },
    ],
    links: [
      { source: "node1", target: "node2" },
      { source: "node2", target: "node3" },
      { source: "node3", target: "node4" },
      { source: "node4", target: "node1" },
    ],
  };

  return (
    <div style={{ width: "100%", height: "600px" }}>
      <ForceGraph2D
        graphData={data}
        nodeLabel="name"
        nodeAutoColorBy="id"
        linkDirectionalArrowLength={6}
        linkDirectionalArrowRelPos={1}
        nodeCanvasObject={(node, ctx, globalScale) => {
          const graphNode = node as GraphNode;
          const label = graphNode.name ?? String(graphNode.id);
          const fontSize = LABEL_FONT_SIZE / globalScale;
          const margin = LABEL_MARGIN / globalScale;
          const nodeValue = typeof graphNode.val === "number" ? graphNode.val : 1;
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
}
