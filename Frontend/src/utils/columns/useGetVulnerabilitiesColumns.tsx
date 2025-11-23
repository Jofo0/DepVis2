import type { VulnerabilitySmallDto } from "@/types/vulnerabilities";
import type { ColumnDef } from "@tanstack/react-table";
import { useMemo } from "react";

export const useGetVulnerabilitiesColumns =
  (): ColumnDef<VulnerabilitySmallDto>[] =>
    useMemo(
      () => [
        {
          accessorKey: "vulnerabilityId",
          header: "Vulnerability ID",
          size: 250,
        },
        {
          accessorKey: "packageName",
          header: "Package Name",
        },
        {
          accessorKey: "severity",
          header: "Severity",
        },
      ],
      []
    );
