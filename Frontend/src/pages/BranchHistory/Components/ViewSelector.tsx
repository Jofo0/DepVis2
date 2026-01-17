import RadioSelect from "@/components/graph/GraphMisc/RadioSelect";

export enum ViewType {
  Vulnerabilities = 0,
  Packages = 1,
  Both = 2,
}

const VIEW_TYPES: {
  text: string;
  value: number;
}[] = [
  { text: "Both", value: ViewType.Both },

  { text: "Vulnerabilities", value: ViewType.Vulnerabilities },
  { text: "Packages", value: ViewType.Packages },
];

type ViewSelectorProps = {
  selected: number;
  onSelect: (names: number) => void;
};

const ViewSelector = ({ selected, onSelect }: ViewSelectorProps) => {
  return (
    <div className="text-gray-700 text-sm">
      View Mode
      <div className="gap-2 flex flex-row pt-1 text-black">
        {VIEW_TYPES.map((parentType) => (
          <div
            className="flex flex-row px-2 py-1"
            key={parentType.text}
            onClick={() => onSelect(parentType.value)}
          >
            <RadioSelect selected={selected === parentType.value} />
            {parentType.text}
          </div>
        ))}
      </div>
    </div>
  );
};

export default ViewSelector;
