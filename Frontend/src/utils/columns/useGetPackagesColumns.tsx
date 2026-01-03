import { SearchFilter } from "@/components/table/SearchFilter";
import HeaderContainer from "@/components/table/HeaderContainer";
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
          <HeaderContainer header="Package">
            <SortButton column={column} />
            <SearchFilter column={column} className="max-w-sm" />
          </HeaderContainer>
        ),
        size: 250,
      },
      {
        accessorKey: "version",
        header: ({ column }: { column: Column<PackageItemDto, unknown> }) => (
          <HeaderContainer header="Version">
            <SortButton column={column} />
          </HeaderContainer>
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
