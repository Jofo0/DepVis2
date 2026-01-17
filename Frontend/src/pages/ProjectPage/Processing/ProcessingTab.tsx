import { Check } from "lucide-react";
import type { HTMLAttributes } from "react";
import clsx from "clsx";
import {
  ProcessStepOrder,
  type Branch,
  type ProcessStep,
} from "@/types/branches";

export type ProcessingTabProps = {
  active: boolean;
  processStep: ProcessStep;
  data: Branch[];
  text: string;

  /** Optional: visually match the step color */
  tone?: "neutral" | "pending" | "success" | "failed";
} & HTMLAttributes<HTMLDivElement>;

const toneStyles = {
  neutral: {
    container: "bg-gray-50 border-gray-200 text-gray-700 hover:bg-gray-100",
    badge: "bg-gray-100 text-gray-700",
    ring: "ring-gray-300",
  },
  pending: {
    container:
      "bg-orange-50 border-orange-200 text-orange-800 hover:bg-orange-100",
    badge: "bg-orange-100 text-orange-800",
    ring: "ring-orange-300",
  },
  success: {
    container: "bg-green-50 border-green-200 text-green-800 hover:bg-green-100",
    badge: "bg-green-100 text-green-800",
    ring: "ring-green-300",
  },
  failed: {
    container: "bg-red-50 border-red-200 text-red-800 hover:bg-red-100",
    badge: "bg-red-100 text-red-800",
    ring: "ring-red-300",
  },
};

const ProcessingTab = ({
  active,
  text,
  data,
  processStep,
  tone = "neutral",
  className,
  ...props
}: ProcessingTabProps) => {
  const completeItems = data.filter(
    (x) =>
      (ProcessStepOrder[x.processStep] == ProcessStepOrder[processStep] &&
        x.processStatus === "Success") ||
      ProcessStepOrder[x.processStep] > ProcessStepOrder[processStep]
  );
  const done = completeItems.length === data.length;

  const styles = toneStyles[tone];

  return (
    <div
      role="button"
      tabIndex={0}
      className={clsx(
        "select-none",
        "flex items-center gap-3",
        "rounded-2xl border-2 px-4 py-3",
        "transition-all duration-200",
        "cursor-pointer",
        "focus:outline-none focus-visible:ring-2 focus-visible:ring-offset-2",
        styles.container,
        active && clsx("shadow-sm ring-2 ring-offset-2", styles.ring),
        className
      )}
      {...props}
    >
      <div className="flex flex-col leading-tight">
        <div className="text-sm font-semibold">{text}</div>
      </div>

      <div className="ml-auto flex items-center">
        {done ? (
          <span className="inline-flex items-center gap-1 rounded-full bg-green-100 px-2 py-1 text-xs font-semibold text-green-800">
            <Check className="h-4 w-4" /> Done
          </span>
        ) : (
          <span
            className={clsx(
              "rounded-full px-2 py-1 text-xs font-semibold tabular-nums",
              styles.badge
            )}
          >
            {completeItems.length}/{data.length}
          </span>
        )}
      </div>
    </div>
  );
};

export default ProcessingTab;
