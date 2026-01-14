import { Check } from "lucide-react";
import type { HtmlHTMLAttributes } from "react";

export type ProcessingTabProps =
  | {
      totalCount: number;
      filtered: number;
      text: string;
    } & HtmlHTMLAttributes<HTMLDivElement>;

const ProcessingTab = ({
  totalCount,
  filtered,
  text,
  ...props
}: ProcessingTabProps) => {
  return (
    <p className="border-2 rounded-2xl p-2 flex flex-row gap-2" {...props}>
      {text} {filtered === totalCount ? <Check /> : `${filtered}/${totalCount}`}
    </p>
  );
};

export default ProcessingTab;
