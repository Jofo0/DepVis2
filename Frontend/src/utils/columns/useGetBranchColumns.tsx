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
        size: 100,
      },
      {
        accessorKey: "packageCount",
        header: ({ column }: { column: Column<BranchDetailed, unknown> }) => (
          <SortButton column={column} header="Package Count" />
        ),
      },
      {
        accessorKey: "vulnerabilityCount",
        header: ({ column }: { column: Column<BranchDetailed, unknown> }) => (
          <SortButton column={column} header="Vulnerability Count" />
        ),
      },
      {
        accessorKey: "commitDate",
        header: ({ column }: { column: Column<BranchDetailed, unknown> }) => (
          <SortButton column={column} header="Commit Date" />
        ),
        cell: ({ row }: { row: Row<BranchDetailed> }) =>
          parseTime(row.original.commitDate),
      },
      {
        accessorKey: "scanDate",
        header: ({ column }: { column: Column<BranchDetailed, unknown> }) => (
          <SortButton column={column} header="Scan Date" />
        ),
        cell: ({ row }: { row: Row<BranchDetailed> }) =>
          parseTime(row.original.scanDate),
      },
    ],
    []
  );
