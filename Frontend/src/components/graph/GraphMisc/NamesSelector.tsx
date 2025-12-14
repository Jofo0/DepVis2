import type { GraphNames } from "../SimpleGraph";
import RadioSelect from "./RadioSelect";

const NAMES: {
  text: string;
  value: GraphNames;
}[] = [
  { text: "Severe Items", value: "severity" },
  { text: "All", value: "all" },
  { text: "None", value: "none" },
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
        {NAMES.map((severity) => (
          <div
            className="flex flex-row px-2 py-1"
            key={severity.text}
            onClick={() => onSelect(severity.value)}
          >
            <RadioSelect selected={selected === severity.value} />
            {severity.text}
          </div>
        ))}
      </div>
    </div>
  );
};

export default NamesSelector;
