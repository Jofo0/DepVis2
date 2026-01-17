import {
  ProcessStatus,
  ProcessStepOrder,
  type Branch,
  type ProcessStep,
} from "@/types/branches";
import { Check, Timer, X } from "lucide-react";
import clsx from "clsx";

export type BranchProcessingCardProps = {
  branch: Branch;
  filter: ProcessStep;
};

const statusConfig = {
  pending: {
    icon: Timer,
    container: "bg-orange-50 border-orange-300 text-orange-700",
    iconBg: "bg-orange-100",
  },
  failed: {
    icon: X,
    container: "bg-red-50 border-red-300 text-red-700",
    iconBg: "bg-red-100",
  },
  success: {
    icon: Check,
    container: "bg-green-50 border-green-300 text-green-700",
    iconBg: "bg-green-100",
  },
  inactive: {
    icon: Check,
    container: "bg-gray-50 border-gray-200 text-gray-400",
    iconBg: "bg-gray-100",
  },
};

const BranchProcessingCard = ({
  branch,
  filter,
}: BranchProcessingCardProps) => {
  const isCurrentStep =
    ProcessStepOrder[branch.processStep] === ProcessStepOrder[filter];

  let statusKey: keyof typeof statusConfig = "inactive";

  if (isCurrentStep) {
    if (branch.processStatus === ProcessStatus.Pending) {
      statusKey = "pending";
    } else if (branch.processStatus === ProcessStatus.Failed) {
      statusKey = "failed";
    } else {
      statusKey = "success";
    }
  }

  const { icon: Icon, container, iconBg } = statusConfig[statusKey];

  return (
    <div
      className={clsx(
        "flex items-center gap-3 max-w-40 p-3 rounded-2xl border-2",
        "transition-all duration-200 hover:shadow-md",
        container
      )}
    >
      <div
        className={clsx(
          "flex h-8 w-8 items-center justify-center rounded-full",
          iconBg
        )}
      >
        <Icon className="h-4 w-4" />
      </div>

      <div className="flex flex-col leading-tight">
        <span className="text-sm font-semibold truncate">{branch.name}</span>
        {isCurrentStep && (
          <span className="text-xs opacity-80">{branch.processStatus}</span>
        )}
      </div>
    </div>
  );
};

export default BranchProcessingCard;
