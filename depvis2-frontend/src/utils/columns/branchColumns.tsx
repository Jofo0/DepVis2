import { SortButton } from "@/components/table/SortButton";
import type { BranchDetailed } from "@/types/branches";
import type { ColumnDef } from "@tanstack/react-table";

export const columns: ColumnDef<BranchDetailed>[] = [
  {
    accessorKey: "name",
    header: "Branch",
  },
  {
    accessorKey: "packageCount",
    header: ({ column }) => (
      <SortButton column={column} header="Package Count" />
    ),
  },
  {
    accessorKey: "vulnerabilityCount",
    header: ({ column }) => (
      <SortButton column={column} header="Vulnerability Count" />
    ),
  },
];
