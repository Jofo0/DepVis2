import { Bar, BarChart, CartesianGrid, XAxis } from "recharts";

import {
  type ChartConfig,
  ChartContainer,
  ChartLegend,
  ChartLegendContent,
  ChartTooltip,
  ChartTooltipContent,
} from "@/components/ui/chart";
import { useGetProjectBranchesDetailedQuery } from "@/store/api/projectsApi";
import { useGetProjectId } from "@/utils/hooks/useGetProjectId";

const chartConfig = {
  packageCount: {
    label: "Package Count",
    color: "#2563eb",
  },
} satisfies ChartConfig;

export const BranchPackageChart = () => {
  const projectId = useGetProjectId();
  const { data = [] } = useGetProjectBranchesDetailedQuery(projectId);

  return (
    <ChartContainer config={chartConfig} className="min-h-[200px] w-full">
      <BarChart accessibilityLayer data={data}>
        <CartesianGrid vertical={false} />
        <XAxis
          dataKey="name"
          tickLine={false}
          tickMargin={10}
          axisLine={false}
          tickFormatter={(value) => value.slice(0, 3)}
        />
        <ChartTooltip content={<ChartTooltipContent />} />
        <ChartLegend content={<ChartLegendContent />} />
        <Bar
          dataKey="packageCount"
          fill="var(--color-packageCount)"
          radius={4}
        />
      </BarChart>
    </ChartContainer>
  );
};
