import type { Severity } from "@/types/packages";

const SEVERITIES: {
  text: string;
  value: Severity;
  borderClass: string;
  bgClass: string;
}[] = [
  {
    text: "Low",
    value: "low",
    borderClass: "border-yellow-500",
    bgClass: "bg-yellow-500",
  },
  {
    text: "Medium",
    value: "medium",
    borderClass: "border-orange-500",
    bgClass: "bg-orange-500",
  },
  {
    text: "High",
    value: "high",
    borderClass: "border-red-500",
    bgClass: "bg-red-500",
  },
  {
    text: "Critical",
    value: "critical",
    borderClass: "border-red-800",
    bgClass: "bg-red-800",
  },
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
            className="flex flex-row px-2 py-1"
            key={severity.text}
            onClick={() =>
              onSelect(severity.value === selected ? undefined : severity.value)
            }
          >
            <div
              className={`relative mr-2 w-6 h-6 border-2 ${severity.borderClass} rounded-2xl`}
            >
              <div
                className={`absolute inset-1 rounded-full ${
                  selected === severity.value ? severity.bgClass : "bg-white"
                }`}
              />
            </div>
            {severity.text}
          </div>
        ))}
      </div>
    </div>
  );
};

export default SeveritySelector;
