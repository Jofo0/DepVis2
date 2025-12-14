type RadioSelectProps = {
  selected: boolean;
  bgColor?: string;
  borderColor?: string;
} & React.HTMLAttributes<HTMLDivElement>;

const RadioSelect = ({
  selected,
  bgColor,
  borderColor,
  ...props
}: RadioSelectProps) => {
  return (
    <div
      className={`relative mr-2 w-6 h-6 border-2 ${
        borderColor ? borderColor : "border-black"
      } rounded-2xl`}
      {...props}
    >
      <div
        className={`absolute inset-1 rounded-full ${
          selected ? (bgColor ? bgColor : "bg-gray-700") : "bg-white"
        }`}
      />
    </div>
  );
};

export default RadioSelect;
