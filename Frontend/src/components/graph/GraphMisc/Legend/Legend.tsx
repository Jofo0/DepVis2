import { CardDescription } from "@/components/ui/card";
import { colors } from "@/theme/colors";
import LegendItem from "./LegendItem";

const Legend = () => {
  return (
    <div className="flex flex-col gap-4 border-2 rounded-2xl bg-white p-4 absolute bottom-10 z-1000 w-4/10">
      <CardDescription>Legend</CardDescription>
      <div className="pl-2 flex flex-row gap-4 items-center">
        <LegendItem title="Project Root" color={colors.deepPurple} />
        <LegendItem title="Critical Severity" color={colors.darkRed} />
        <LegendItem title="High Severity" color={colors.red} />
        <LegendItem title="Medium Severity" color={colors.orange} />
        <LegendItem title="Low Severity" color={colors.yellow} />
      </div>
    </div>
  );
};

export default Legend;
