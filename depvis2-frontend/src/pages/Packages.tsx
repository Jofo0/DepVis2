import BranchSelector from "@/components/BranchSelector";
import { DataTable } from "@/components/table/DataTable";
import { useLazyGetPackagesQuery } from "@/store/api/projectsApi";
import { columns } from "@/utils/columns/packagesColumns";
import { useBranch } from "@/utils/hooks/BranchProvider";
import { toODataOrderBy } from "@/utils/odataHelper";
import {
  useReactTable,
  getSortedRowModel,
  getCoreRowModel,
  type SortingState,
} from "@tanstack/react-table";
import { useEffect, useState } from "react";

const Packages = () => {
  const { branch } = useBranch();
  const [sorting, setSorting] = useState<SortingState>([]);

  const [fetchPackages, { data = [], isLoading }] = useLazyGetPackagesQuery();

  const table = useReactTable({
    data,
    columns,
    onSortingChange: setSorting,
    getSortedRowModel: getSortedRowModel(),
    state: {
      sorting,
    },
    manualSorting: true,
    getCoreRowModel: getCoreRowModel(),
  });

  useEffect(() => {
    if (branch) {
      fetchPackages({
        id: branch.id,
        odata: toODataOrderBy(sorting),
      });
    }
  }, [branch]);

  return (
    <div className="flex flex-col gap-3 w-full h-full py-8">
      <div className="flex flex-row gap-10 w-full h-full justify-evenly">
        <BranchSelector />
        <div className="h-max-full w-full">
          <DataTable
            isLoading={isLoading}
            className="min-h-[calc(100vh-8.5rem)] max-h-[calc(100vh-8.5rem)]"
            table={table}
          />
        </div>
      </div>
    </div>
  );
};

export default Packages;
