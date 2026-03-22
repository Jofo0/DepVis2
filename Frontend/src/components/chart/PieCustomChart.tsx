import { PieChart, Pie, Legend } from "recharts";
import { Card, CardDescription, CardHeader } from "@/components/ui/card";
import type { NameCount } from "@/types/packages";
import str from "string-to-color";
import { Info } from "lucide-react";
import {
  Tooltip as UiTooltip,
  TooltipContent as UiTooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from "@/components/ui/tooltip";
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
    <Card className={`w-full ${className} overflow-hidden relative`}>
      <div className="w-full flex flex-row justify-between items-center ">
        <CardHeader className="w-1/2">{title}</CardHeader>
      </div>

      <div className="h-full w-full flex items-start justify-center">
        {isLoading ? (
          <ChartLoader />
        ) : (
          <ChartContainer
            config={chartConfig}
            className="h-11/12 w-11/12 relative "
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
              <Legend />
            </PieChart>
          </ChartContainer>
        )}
      </div>
      <div className="absolute bottom-5 left-5">
        <CardDescription className="h-full pt-1">
          {filteredBy && (
            <div className="flex flex-row gap-1">
              Currently filtered by:
              <div className="text-black">{filteredBy}</div>
            </div>
          )}
        </CardDescription>
      </div>
      <div className="absolute top-5 right-3.5">
        <TooltipProvider>
          <UiTooltip>
            <TooltipTrigger asChild>
              <Info className="h-4 w-4 text-gray-500" />
            </TooltipTrigger>
            <UiTooltipContent>
              <p>Click on any slice to filter by the value</p>
            </UiTooltipContent>
          </UiTooltip>
        </TooltipProvider>
      </div>
    </Card>
  );
};

export const ChartLoader = () => {
  return (
    <div className="h-full place-self-center self-center flex flex-col items-center justify-center gap-2">
      <div className="text-2xl">Loading</div>
      <div className="loader" />
    </div>
  );
};
