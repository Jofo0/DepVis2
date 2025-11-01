import type { PackageDetailedDto } from "@/types/packages";
import type { ColumnDef } from "@tanstack/react-table";
import { useMemo } from "react";

export const useGetPackagesColumns = (): ColumnDef<PackageDetailedDto>[] =>
  useMemo(
    () => [
      {
        accessorKey: "name",
        header: "Package",
      },
      {
        accessorKey: "version",
        header: "Version",
      },
      {
        accessorKey: "ecosystem",
        header: "Ecosystem",
      },
      {
        accessorKey: "vulnerable",
        header: "Vulnerable",
      },
    ],
    []
  );
