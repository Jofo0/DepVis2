type LegendItemProps = {
  title: string;
  color: string;
};

const LegendItem = ({ title, color }: LegendItemProps) => {
  return (
    <div className="flex flex-row items-center gap-1.5">
      <div>{title}</div>
      <div className="w-4 h-4 rounded-sm" style={{ backgroundColor: color }} />
    </div>
  );
};

export default LegendItem;
