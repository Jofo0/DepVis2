import { SortButton } from "@/components/table/SortButton";
import type { BranchDetailed } from "@/types/branches";
import type { Column, ColumnDef, Row } from "@tanstack/react-table";
import { parseTime } from "../parseTime";
import { useMemo } from "react";
import HeaderContainer from "@/components/table/HeaderContainer";
import { Download, Loader, Play } from "lucide-react";
import { Button } from "@/components/ui/button";
import {
  Tooltip,
  TooltipContent,
  TooltipTrigger,
} from "@/components/ui/tooltip";

export const useGetBranchColumns = (
  handleReprocessCallback: (id: string) => void,
  handleDownloadCallback: (id: string) => void,
  loadingId: string | null,
  downloadingId: string | null,
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
        header: () => "Actions",
        cell: ({ row }: { row: Row<BranchDetailed> }) => {
          const rowId = row.original.id;
          const isRowLoading = loadingId === rowId;
          const isRowDownloading = downloadingId === rowId;

          return (
            <div className="flex flex-row gap-2">
              <Tooltip>
                <TooltipContent>Reprocess</TooltipContent>
                <TooltipTrigger asChild>
                  <Button
                    variant="outline"
                    onClick={() => handleReprocessCallback(rowId)}
                    disabled={isRowLoading}
                  >
                    {isRowLoading ? (
                      <Loader className="animate-spin" />
                    ) : (
                      <Play />
                    )}
                  </Button>
                </TooltipTrigger>
              </Tooltip>
              <Tooltip>
                <TooltipContent>Download</TooltipContent>
                <TooltipTrigger asChild>
                  <Button
                    variant="outline"
                    onClick={() => handleDownloadCallback(rowId)}
                    disabled={isRowDownloading}
                  >
                    {isRowDownloading ? (
                      <Loader className="animate-spin" />
                    ) : (
                      <Download />
                    )}
                  </Button>
                </TooltipTrigger>
              </Tooltip>
            </div>
          );
        },
      },
    ],
    [loadingId, downloadingId, handleReprocessCallback, handleDownloadCallback],
  );
};
