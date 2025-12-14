import { colors } from "@/theme/colors";
import type { Severity } from "@/types/packages";

export const riskToColor = (risk: Severity): string => {
  switch (risk.toLowerCase()) {
    case "low":
      return colors.yellow;
    case "medium":
      return colors.orange;
    case "high":
      return colors.red;
    case "critical":
      return colors.darkRed;
    default:
      return colors.green;
  }
};

export const severityToBgColor: Record<Severity, string> = {
  low: "bg-yellow-500",
  medium: "bg-orange-500",
  high: "bg-red-500",
  critical: "bg-red-900",
  None: "",
};

export const severityToBorderColor: Record<Severity, string> = {
  low: "border-yellow-500",
  medium: "border-orange-500",
  high: "border-red-500",
  critical: "border-red-900",
  None: "",
};
