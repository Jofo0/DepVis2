import { Bar, BarChart, CartesianGrid, XAxis, YAxis } from "recharts";

import {
  type ChartConfig,
  ChartContainer,
  ChartTooltip,
  ChartTooltipContent,
} from "@/components/ui/chart";
import { Card, CardHeader } from "../ui/card";
import { ChartLoader } from "./PieCustomChart";

type DataItem = {
  [key: string]: string | number;
};

type BranchPackageChartProps = {
  data: DataItem[]; // Use the DataItem type for the data array
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

  const maxLabelLength = Math.max(
    ...data.map((item) => item[xKey].toString().length)
  );
  const calculatedHeight = Math.min(120, maxLabelLength * 10); // Adjust multiplier based on your label length

  return (
    <Card className={`p-4  ${className}`}>
      <CardHeader>{yLabel}</CardHeader>
      <ChartContainer config={chartConfig} className="h-full overflow-auto">
        {isLoading ? (
          <ChartLoader />
        ) : (
          <BarChart accessibilityLayer data={data}>
            <CartesianGrid vertical={false} />
            <XAxis
              dataKey={xKey}
              tickLine={false}
              angle={45}
              textAnchor="start"
              axisLine={false}
              height={calculatedHeight}
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
