import { SortButton } from "@/components/table/SortButton";
import type { BranchDetailed } from "@/types/branches";
import type { Column, ColumnDef, Row } from "@tanstack/react-table";
import { parseTime } from "../parseTime";
import { useMemo } from "react";
import HeaderContainer from "@/components/table/HeaderContainer";
import { Loader, Play } from "lucide-react";
import { Button } from "@/components/ui/button";

export const useGetBranchColumns = (
  handleReprocessCallback: (id: string) => void,
  loadingId: string | null,
): ColumnDef<BranchDetailed>[] => {
  return useMemo(
    () => [
      {
        accessorKey: "name",
        header: "Branch",
        size: 100,
      },
      {
        accessorKey: "packageCount",
        header: ({ column }: { column: Column<BranchDetailed, unknown> }) => (
          <HeaderContainer header="Package Count">
            <SortButton column={column} />
          </HeaderContainer>
        ),
      },
      {
        accessorKey: "vulnerabilityCount",
        header: ({ column }: { column: Column<BranchDetailed, unknown> }) => (
          <HeaderContainer header="Vulnerability Count">
            <SortButton column={column} />
          </HeaderContainer>
        ),
      },
      {
        accessorKey: "commitDate",
        header: ({ column }: { column: Column<BranchDetailed, unknown> }) => (
          <HeaderContainer header="Commit Date">
            <SortButton column={column} />
          </HeaderContainer>
        ),
        cell: ({ row }: { row: Row<BranchDetailed> }) =>
          parseTime(row.original.commitDate),
      },
      {
        accessorKey: "scanDate",
        header: ({ column }: { column: Column<BranchDetailed, unknown> }) => (
          <HeaderContainer header="Scan Date">
            <SortButton column={column} />
          </HeaderContainer>
        ),
        cell: ({ row }: { row: Row<BranchDetailed> }) =>
          parseTime(row.original.scanDate),
      },
      {
        meta: { disableTooltip: true },
        id: "rerunAction",
        header: () => "Rerun Scan",
        cell: ({ row }: { row: Row<BranchDetailed> }) => {
          const rowId = row.original.id;
          const isRowLoading = loadingId === rowId;

          return (
            <Button
              variant="outline"
              onClick={() => handleReprocessCallback(rowId)}
              disabled={isRowLoading}
            >
              {isRowLoading ? <Loader className="animate-spin" /> : <Play />}
            </Button>
          );
        },
      },
    ],
    [loadingId, handleReprocessCallback],
  );
};
