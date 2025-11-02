import { PieChart, Pie, Tooltip, Legend } from "recharts";
import { Card, CardHeader } from "@/components/ui/card";
import type { NameCount } from "@/types/packages";
import str from "string-to-color";
import {
  ChartContainer,
  ChartTooltip,
  ChartTooltipContent,
  type ChartConfig,
} from "../ui/chart";

type PiePackagesChartProps = {
  pies: (NameCount & { color?: string })[];
  title?: string;
  className?: string;
};

export const PieCustomChart = ({
  pies,
  title = "Packages Overview",
  className = "",
}: PiePackagesChartProps) => {
  const chartConfig = pies.reduce((acc, x) => {
    acc[x.name] = {
      label: x.name,
      color: str(x.name),
    };
    return acc;
  }, {} as ChartConfig);

  return (
    <Card className={`p-4 w-full ${className} overflow-hidden`}>
      <CardHeader className="text-lg font-semibold">{title}</CardHeader>
      <div className="h-full w-full">
        <ChartContainer
          config={chartConfig}
          className="aspect-square max-h-5/6 min-h-5/6 w-full"
        >
          <PieChart>
            <ChartTooltip
              cursor={false}
              content={<ChartTooltipContent hideLabel />}
            />
            <Pie
              data={pies.map((x) => {
                return { ...x, fill: `var(--color-${x.name})` };
              })}
              dataKey="count"
              nameKey="name"
              label
            />
            <Tooltip />
            <Legend />
          </PieChart>
        </ChartContainer>
      </div>
    </Card>
  );
};
