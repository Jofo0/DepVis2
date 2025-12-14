import { SortButton } from "@/components/table/SortButton";
import type { VulnerabilitySmallDto } from "@/types/vulnerabilities";
import type { Column, ColumnDef } from "@tanstack/react-table";
import { useMemo } from "react";

export const useGetVulnerabilitiesColumns =
  (): ColumnDef<VulnerabilitySmallDto>[] =>
    useMemo(
      () => [
        {
          accessorKey: "vulnerabilityId",
          header: ({
            column,
          }: {
            column: Column<VulnerabilitySmallDto, unknown>;
          }) => <SortButton column={column} header="Vulnerability Id" />,
          size: 250,
        },
        {
          accessorKey: "packageName",
          header: ({
            column,
          }: {
            column: Column<VulnerabilitySmallDto, unknown>;
          }) => <SortButton column={column} header="Package Name" />,
        },
        {
          accessorKey: "severity",
          header: ({
            column,
          }: {
            column: Column<VulnerabilitySmallDto, unknown>;
          }) => <SortButton column={column} header="Severity" />,
        },
      ],
      []
    );
