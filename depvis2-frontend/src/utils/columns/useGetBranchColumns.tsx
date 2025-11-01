import { SortButton } from "@/components/table/SortButton";
import type { BranchDetailed } from "@/types/branches";
import type { Column, ColumnDef, Row } from "@tanstack/react-table";
import { parseTime } from "../parseTime";
import { useMemo } from "react";

export const useGetBranchColumns = (): ColumnDef<BranchDetailed>[] =>
  useMemo(
    () => [
      {
        accessorKey: "name",
        header: "Branch",
        size: 200,
      },
      {
        accessorKey: "packageCount",
        header: ({ column }: { column: Column<BranchDetailed, unknown> }) => (
          <SortButton column={column} header="Package Count" />
        ),
        size: 50,
      },
      {
        accessorKey: "vulnerabilityCount",
        header: ({ column }: { column: Column<BranchDetailed, unknown> }) => (
          <SortButton column={column} header="Vulnerability Count" />
        ),
        size: 200,
      },
      {
        header: "Commit Date",
        accessorKey: "commitDate",
        cell: ({ row }: { row: Row<BranchDetailed> }) =>
          parseTime(row.original.commitDate),
        size: 200,
      },
      {
        accessorKey: "scanDate",
        header: "Scan Date",
        size: 200,
        cell: ({ row }: { row: Row<BranchDetailed> }) =>
          parseTime(row.original.scanDate),
      },
    ],
    []
  );
