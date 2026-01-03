import { PieChart, Pie, Tooltip, Legend } from "recharts";
import { Card, CardDescription, CardHeader } from "@/components/ui/card";
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
  onSliceClick?: (name: string) => void;
  isLoading?: boolean;
  filteredBy?: string;
};

export const PieCustomChart = ({
  pies,
  title = "Packages Overview",
  className = "",
  onSliceClick,
  isLoading = false,
  filteredBy,
}: PiePackagesChartProps) => {
  const chartConfig = pies.reduce((acc, x) => {
    acc[x.name] = {
      label: x.name,
      color: x.color ?? str(x.name),
    };
    return acc;
  }, {} as ChartConfig);

  const handleSliceClick = (data: { name: string }) => {
    const name = data?.name;
    if (name && onSliceClick) onSliceClick(name);
  };

  return (
    <Card className={`w-full ${className} overflow-hidden`}>
      <CardHeader>{title}</CardHeader>
      <div className="flex flex-row justify-between">
        <CardDescription className="px-7">
          Click on any slice to filter by the value
        </CardDescription>
        <CardDescription className="px-7">
          {filteredBy && (
            <div className="flex flex-row gap-1">
              Currently filtered by:
              <div className="text-black">{filteredBy}</div>
            </div>
          )}
        </CardDescription>
      </div>

      <div className="h-full w-full">
        {isLoading ? (
          <ChartLoader />
        ) : (
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
                data={pies.map((x) => ({
                  ...x,
                  fill: `var(--color-${x.name})`,
                }))}
                dataKey="count"
                nameKey="name"
                label
                onClick={handleSliceClick}
                style={{ cursor: onSliceClick ? "pointer" : "default" }}
              />
              <Tooltip />
              <Legend />
            </PieChart>
          </ChartContainer>
        )}
      </div>
    </Card>
  );
};

export const ChartLoader = () => {
  return (
    <div className="h-full place-self-center self-center flex flex-col items-center justify-center">
      <div className="loader" />
    </div>
  );
};
