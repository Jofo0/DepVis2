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
