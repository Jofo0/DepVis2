import { DataTable } from "@/components/table/DataTable";
import { useGetPackagesQuery } from "@/store/api/projectsApi";
import { columns } from "@/utils/columns/packagesColumns";
import { useGetProjectId } from "@/utils/hooks/useGetProjectId";
import { toODataOrderBy } from "@/utils/odataHelper";
import {
  useReactTable,
  getSortedRowModel,
  getCoreRowModel,
  type SortingState,
} from "@tanstack/react-table";
import { useState } from "react";

const Packages = () => {
  const projectId = useGetProjectId();
  const [sorting, setSorting] = useState<SortingState>([]);

  const { data = [], isLoading } = useGetPackagesQuery({
    id: projectId,
    odata: toODataOrderBy(sorting),
  });

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
  return (
    <div className="flex flex-col gap-3 w-full h-full py-8">
      <div className="flex flex-row gap-10 w-full h-full justify-evenly">
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
