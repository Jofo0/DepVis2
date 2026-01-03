import { Bar, BarChart, CartesianGrid, XAxis, YAxis } from "recharts";

import {
  type ChartConfig,
  ChartContainer,
  ChartTooltip,
  ChartTooltipContent,
} from "@/components/ui/chart";
import { Card, CardHeader } from "../ui/card";
import { ChartLoader } from "./PieCustomChart";

type BranchPackageChartProps = {
  data: unknown[];
  xKey: string;
  yKey: string;
  yLabel: string;
  color?: string;
  className?: string;
  isLoading?: boolean;
};

export const XYChart = ({
  data,
  xKey,
  yKey,
  yLabel,
  color = "#2563eb",
  className,
  isLoading = false,
}: BranchPackageChartProps) => {
  const chartConfig = {
    [yKey]: {
      label: yLabel,
      color: color,
    },
  } satisfies ChartConfig;

  return (
    <Card className={`p-4 w-full ${className}`}>
      <CardHeader>{yLabel}</CardHeader>
      <ChartContainer config={chartConfig} className="h-full">
        {isLoading ? (
          <ChartLoader />
        ) : (
          <BarChart accessibilityLayer data={data}>
            <CartesianGrid vertical={false} />
            <XAxis
              dataKey={xKey}
              tickLine={false}
              tickMargin={10}
              axisLine={false}
              tickFormatter={(value) => value.slice(0, 4)}
            />
            <YAxis dataKey={yKey} tickMargin={10} axisLine={false} />
            <ChartTooltip content={<ChartTooltipContent />} />
            <Bar dataKey={yKey} fill={`var(--color-${yKey})`} radius={4} />
          </BarChart>
        )}
      </ChartContainer>
    </Card>
  );
};
