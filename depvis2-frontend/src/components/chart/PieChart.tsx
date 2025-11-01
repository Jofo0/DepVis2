import {
  PieChart,
  Pie,
  Cell,
  Tooltip,
  Legend,
  ResponsiveContainer,
} from "recharts";
import { Card, CardHeader, CardContent } from "@/components/ui/card";

type PieData = {
  name: string;
  value: number;
  color?: string;
};

type PiePackagesChartProps = {
  pies: PieData[];
  title?: string;
  className?: string;
};

export const PiePackagesChart = ({
  pies,
  title = "Packages Overview",
  className = "",
}: PiePackagesChartProps) => {
  return (
    <Card className={`p-4 w-full ${className}`}>
      <CardHeader className="text-lg font-semibold">{title}</CardHeader>
      <CardContent className="h-64">
        <ResponsiveContainer width="100%" height="100%">
          <PieChart>
            <Pie
              data={pies}
              dataKey="value"
              nameKey="name"
              cx="50%"
              cy="50%"
              outerRadius={80}
              label
            >
              {pies.map((entry, index) => (
                <Cell
                  key={`cell-${index}`}
                  fill={entry.color ?? "#2563eb"}
                  stroke="#fff"
                  strokeWidth={2}
                />
              ))}
            </Pie>
            <Tooltip />
            <Legend />
          </PieChart>
        </ResponsiveContainer>
      </CardContent>
    </Card>
  );
};
