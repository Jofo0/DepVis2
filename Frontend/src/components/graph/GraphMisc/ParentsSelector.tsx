const PARENT_TYPES: {
  text: string;
  value: boolean;
}[] = [
  { text: "All", value: true },
  { text: "One Parent", value: false },
];

type ParentsSelectorProps = {
  selected: boolean;
  onSelect: (names: boolean) => void;
};

const ParentsSelector = ({ selected, onSelect }: ParentsSelectorProps) => {
  return (
    <div className="text-gray-700 text-sm">
      Show Parents
      <div className="gap-2 flex flex-row pt-1 text-black">
        {PARENT_TYPES.map((parentType) => (
          <div
            className="flex flex-row px-2 py-1"
            key={parentType.text}
            onClick={() => onSelect(parentType.value)}
          >
            <div className="relative mr-2 w-6 h-6 border-2 border-black rounded-2xl">
              <div
                className={`absolute inset-1 rounded-full ${
                  selected === parentType.value ? "bg-gray-700" : "bg-white"
                }`}
              />
            </div>

            {parentType.text}
          </div>
        ))}
      </div>
    </div>
  );
};

export default ParentsSelector;
