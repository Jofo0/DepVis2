import { SortButton } from "@/components/table/SortButton";
import type { BranchDetailed } from "@/types/branches";
import type { Column, ColumnDef, Row } from "@tanstack/react-table";
import { parseTime } from "../parseTime";
import { useCallback, useMemo, useState } from "react";
import HeaderContainer from "@/components/table/HeaderContainer";
import { Loader, Play } from "lucide-react";
import { Button } from "@/components/ui/button";
import { useReprocessBranchMutation } from "@/store/api/branchesApi";
import { useGetProjectId } from "../hooks/useGetProjectId";

export const useGetBranchColumns = (): ColumnDef<BranchDetailed>[] => {
  const [reprocess] = useReprocessBranchMutation();
  const projectId = useGetProjectId();
  const [loadingId, setLoadingId] = useState<string | null>(null);

  const handleReprocess = useCallback(
    async (id: string) => {
      try {
        setLoadingId(id);
        await reprocess({ id, projectId }).unwrap();
      } finally {
        setLoadingId(null);
      }
    },
    [reprocess, projectId],
  );

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
              onClick={() => handleReprocess(rowId)}
              disabled={isRowLoading}
            >
              {isRowLoading ? <Loader className="animate-spin" /> : <Play />}
            </Button>
          );
        },
      },
    ],
    [loadingId, handleReprocess],
  );
};
