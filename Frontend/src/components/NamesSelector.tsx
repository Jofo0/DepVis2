import type { GraphNames } from "./graph/SimpleGraph";

const SEVERITIES: {
  text: string;
  value: GraphNames;
}[] = [
  { text: "None", value: "none" },
  { text: "All", value: "all" },
  { text: "Severe Items", value: "severity" },
];

type NamesSelectorProps = {
  selected: GraphNames;
  onSelect: (names: GraphNames) => void;
};

const NamesSelector = ({ selected, onSelect }: NamesSelectorProps) => {
  return (
    <div className="text-gray-700 text-sm">
      Show Names
      <div className="gap-2 flex flex-row pt-1 text-black">
        {SEVERITIES.map((severity) => (
          <div
            className="flex flex-row px-2 py-1"
            key={severity.text}
            onClick={() => onSelect(severity.value)}
          >
            <div className="relative mr-2 w-6 h-6 border-2 border-black rounded-2xl">
              <div
                className={`absolute inset-1 rounded-full ${
                  selected === severity.value ? "bg-gray-700" : "bg-white"
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

export default NamesSelector;
