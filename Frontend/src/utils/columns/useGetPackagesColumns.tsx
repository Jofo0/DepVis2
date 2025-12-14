import { SortButton } from "@/components/table/SortButton";
import type { PackageItemDto } from "@/types/packages";
import type { Column, ColumnDef } from "@tanstack/react-table";
import { useMemo } from "react";

export const useGetPackagesColumns = (): ColumnDef<PackageItemDto>[] =>
  useMemo(
    () => [
      {
        accessorKey: "name",
        header: ({ column }: { column: Column<PackageItemDto, unknown> }) => (
          <SortButton column={column} header="Package" />
        ),
        size: 250,
      },
      {
        accessorKey: "version",
        header: ({ column }: { column: Column<PackageItemDto, unknown> }) => (
          <SortButton column={column} header="Version" />
        ),
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
