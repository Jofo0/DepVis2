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
import useTranslation from "@/utils/hooks/useTranslation";

type PiePackagesChartProps = {
  pies: (NameCount & { color?: string })[];
  title?: string;
  className?: string;
  onSliceClick?: (name: string) => void;
};

export const PieCustomChart = ({
  pies,
  title = "Packages Overview",
  className = "",
  onSliceClick,
}: PiePackagesChartProps) => {
  const t = useTranslation();
  const chartConfig = pies.reduce((acc, x) => {
    acc[x.name] = {
      label: x.name,
      color: x.color ?? str(x.name),
    };
    return acc;
  }, {} as ChartConfig);

  const handleSliceClick = (data) => {
    const name = data?.name ?? data?.payload?.name;
    if (name && onSliceClick) onSliceClick(name as string);
  };

  return (
    <Card className={`p-4 w-full ${className} overflow-hidden`}>
      <CardHeader className="text-lg font-semibold">{title}</CardHeader>
      <CardDescription className="px-7">
        {"Click on any slice to filter by the value"}
      </CardDescription>

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
              data={pies.map((x) => ({ ...x, fill: `var(--color-${x.name})` }))}
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
      </div>
    </Card>
  );
};
