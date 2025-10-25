import type { BranchDetailed } from "@/types/branches";
import type { ColumnDef } from "@tanstack/react-table";

export const columns: ColumnDef<BranchDetailed>[] = [
  {
    accessorKey: "name",
    header: "Name",
  },
  {
    accessorKey: "packageCount",
    header: "Package Count",
  },
  {
    accessorKey: "vulnerabilityCount",
    header: "Vulnerability Count",
  },
];
