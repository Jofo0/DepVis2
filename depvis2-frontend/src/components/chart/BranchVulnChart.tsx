import { Bar, BarChart, CartesianGrid, XAxis, YAxis } from "recharts";

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
  vulnerabilityCount: {
    label: "Vulnerability Count",
    color: "#d12c2c",
  },
} satisfies ChartConfig;

export const BranchVulnChart = () => {
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

        <YAxis
          dataKey="vulnerabilityCount"
          tickMargin={10}
          axisLine={false}
          label={{
            value: "Vulnerabilities",
            angle: -90,
            position: "insideLeft",
          }}
        />
        <ChartTooltip content={<ChartTooltipContent />} />
        <ChartLegend content={<ChartLegendContent />} />
        <Bar
          dataKey="vulnerabilityCount"
          fill="var(--color-vulnerabilityCount)"
          radius={4}
        />
      </BarChart>
    </ChartContainer>
  );
};
