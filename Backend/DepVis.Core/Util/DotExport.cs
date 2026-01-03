namespace DepVis.Core.Util;

using System.Text;
using DepVis.Core.Dtos;

public static class DotExport
{
    public static string ToDot(GraphDataDto graph, string graphName = "deps")
    {
        var sb = new StringBuilder();

        sb.AppendLine($"digraph {DotId(graphName)} {{");
        sb.AppendLine("  node [shape=box, style=filled, fontname=\"Arial\"];");
        sb.AppendLine("  edge [fontname=\"Arial\"];");
        sb.AppendLine();

        foreach (var p in graph.Packages)
        {
            var nodeId = DotId(p.Id.ToString("N"));
            var label = DotString(p.Name);

            var (fill, font) = SeverityColors(p.Severity);

            sb.Append("  ").Append(nodeId).Append(" [label=").Append(label);

            if (fill is not null)
                sb.Append(", fillcolor=").Append(DotString(fill));

            if (font is not null)
                sb.Append(", fontcolor=").Append(DotString(font));

            sb.AppendLine("];");
        }

        sb.AppendLine();

        foreach (var r in graph.Relationships)
        {
            var fromId = DotId(r.From.ToString("N"));
            var toId = DotId(r.To.ToString("N"));

            sb.AppendLine($"  {fromId} -> {toId};");
        }

        sb.AppendLine("}");
        return sb.ToString();
    }

    private static string DotId(string id) => $"\"{Escape(id)}\"";

    private static string DotString(string s) => $"\"{Escape(s)}\"";

    private static string Escape(string s) =>
        s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "").Replace("\n", "\\n");

    private static (string? fillColor, string? fontColor) SeverityColors(string severity)
    {
        return severity.Trim().ToLowerInvariant() switch
        {
            "critical" => ("#8B0000", "#FFFFFF"),
            "high" => ("#FF4D4D", "#000000"),
            "medium" => ("#FFA500", "#000000"),
            "low" => ("#FFD700", "#000000"),
            _ => (null, null),
        };
    }
}
