import type { PackageDetailedDto } from "@/types/packages";
import type { ColumnDef } from "@tanstack/react-table";

export const columns: ColumnDef<PackageDetailedDto>[] = [
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
];
