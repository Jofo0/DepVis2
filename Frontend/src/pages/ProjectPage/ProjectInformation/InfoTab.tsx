export type InfoTabProps = {
  title: string;
  info: string | number;
};

const InfoTab = ({ title, info }: InfoTabProps) => {
  return (
    <div className="rounded-2xl border bg-muted/30 p-3">
      <div className="text-xs text-muted-foreground">{title}</div>
      <div className="text-lg font-semibold">{info}</div>
    </div>
  );
};

export default InfoTab;
