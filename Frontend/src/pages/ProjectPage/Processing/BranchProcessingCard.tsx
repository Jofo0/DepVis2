import {
  ProcessStatus,
  ProcessStepOrder,
  type Branch,
  type ProcessStep,
} from "@/types/branches";
import { Check, Timer, X } from "lucide-react";

export type BranchProcessingCardProps = {
  branch: Branch;
  filter: ProcessStep;
};

const BranchProcessingCard = ({
  branch,
  filter,
}: BranchProcessingCardProps) => {
  let icon;
  if (ProcessStepOrder[branch.processStep] === ProcessStepOrder[filter]) {
    if (branch.processType === ProcessStatus.Pending) {
      icon = <Timer />;
    } else if (branch.processType === ProcessStatus.Failed) {
      icon = <X />;
    } else {
      icon = <Check />;
    }
  } else {
    icon = <Check />;
  }
  return (
    <div className="border-2 rounded-2xl p-2 flex flex-row gap-2">
      {branch.name} {icon}
    </div>
  );
};

export default BranchProcessingCard;
