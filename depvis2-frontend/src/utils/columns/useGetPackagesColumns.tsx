import type { PackageItemDto } from "@/types/packages";
import type { ColumnDef } from "@tanstack/react-table";
import { useMemo } from "react";

export const useGetPackagesColumns = (): ColumnDef<PackageItemDto>[] =>
  useMemo(
    () => [
      {
        accessorKey: "name",
        header: "Package",
        size: 250,
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
