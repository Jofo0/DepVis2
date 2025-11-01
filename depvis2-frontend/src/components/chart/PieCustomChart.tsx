import { PieChart, Pie, Tooltip, Legend } from "recharts";
import { Card, CardContent, CardHeader } from "@/components/ui/card";
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
    <Card className={`p-4 w-full ${className}`}>
      <CardHeader className="text-lg font-semibold">{title}</CardHeader>
      <CardContent className="flex-1 pb-0">
        <ChartContainer
          config={chartConfig}
          className="mx-auto aspect-square max-h-[250px]"
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
              outerRadius={80}
              label
            />
            <Tooltip />
            <Legend />
          </PieChart>
        </ChartContainer>
      </CardContent>
    </Card>
  );
};
