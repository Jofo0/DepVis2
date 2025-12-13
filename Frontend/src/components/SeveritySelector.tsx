import type { Severity } from "@/types/packages";
import { Circle, CirclePlus } from "lucide-react";

const SEVERITIES: {
  text: string;
  color: string;
  value: Severity;
}[] = [
  { text: "Low", color: "text-yellow-500", value: "low" },
  { text: "Medium", color: "text-orange-500", value: "medium" },
  { text: "High", color: "text-red-500", value: "high" },
  { text: "Critical", color: "text-red-900", value: "critical" },
];

type SeveritySelectorProps = {
  selected?: Severity;
  onSelect: (severity?: Severity) => void;
};

const SeveritySelector = ({ selected, onSelect }: SeveritySelectorProps) => {
  return (
    <div className="text-gray-700 text-sm">
      Severity Filter
      <div className="gap-2 flex flex-row pt-1 text-black">
        {SEVERITIES.map((severity) => (
          <div
            className=" px-2 py-1 "
            key={severity.text}
            onClick={() =>
              onSelect(severity.value === selected ? undefined : severity.value)
            }
          >
            {selected === severity.value ? (
              <CirclePlus className={`${severity.color} inline-block mr-2`} />
            ) : (
              <Circle className={`${severity.color} inline-block mr-2`} />
            )}
            {severity.text}
          </div>
        ))}
      </div>
    </div>
  );
};

export default SeveritySelector;
