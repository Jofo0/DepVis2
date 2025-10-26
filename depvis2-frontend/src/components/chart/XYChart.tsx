import { Bar, BarChart, CartesianGrid, XAxis, YAxis } from "recharts";

import {
  type ChartConfig,
  ChartContainer,
  ChartLegend,
  ChartLegendContent,
  ChartTooltip,
  ChartTooltipContent,
} from "@/components/ui/chart";

type BranchPackageChartProps = {
  data: unknown[];
  xKey: string;
  yKey: string;
  yLabel: string;
  color?: string;
};

export const XYChart = ({
  data,
  xKey,
  yKey,
  yLabel,
  color = "#2563eb",
}: BranchPackageChartProps) => {
  const chartConfig = {
    [yKey]: {
      label: yLabel,
      color: color,
    },
  } satisfies ChartConfig;

  return (
    <ChartContainer config={chartConfig} className="min-h-1/2 w-full">
      <BarChart accessibilityLayer data={data}>
        <CartesianGrid vertical={false} />
        <XAxis
          dataKey={xKey}
          tickLine={false}
          tickMargin={10}
          axisLine={false}
          tickFormatter={(value) => value.slice(0, 4)}
        />
        <YAxis
          dataKey={yKey}
          tickMargin={10}
          axisLine={false}
          label={{
            value: yLabel,
            angle: -90,
            position: "insideLeft",
          }}
        />
        <ChartTooltip content={<ChartTooltipContent />} />
        <ChartLegend content={<ChartLegendContent />} />
        <Bar dataKey={yKey} fill={`var(--color-${yKey})`} radius={4} />
      </BarChart>
    </ChartContainer>
  );
};
