import { DataTable } from "@/components/table/DataTable";
import { columns } from "@/utils/columns/branchColumns";
import { useGetProjectId } from "@/utils/hooks/useGetProjectId";
import { useGetProjectBranchesDetailedQuery } from "@/store/api/projectsApi";
import {
  useReactTable,
  getSortedRowModel,
  getCoreRowModel,
  type SortingState,
} from "@tanstack/react-table";
import { useState } from "react";
import { XYChart } from "@/components/chart/XYChart";

export const toODataOrderBy = (sorting: SortingState): string => {
  if (!sorting?.length) return "";
  const orderBy = sorting
    .map((s) => `${s.id} ${s.desc ? "desc" : "asc"}`)
    .join(", ");
  return `$orderby=${encodeURIComponent(orderBy)}`;
};

const Branches = () => {
  const projectId = useGetProjectId();
  const [sorting, setSorting] = useState<SortingState>([]);

  const { data = [], isLoading } = useGetProjectBranchesDetailedQuery({
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
        <div className="flex flex-col gap-4 w-1/2 h-full">
          <XYChart
            data={data}
            xKey="name"
            yKey="packageCount"
            yLabel="Package Count"
          />
          <XYChart
            data={data}
            xKey="name"
            yKey="vulnerabilityCount"
            yLabel="Vulnerabilities Count"
            color="#d12c2c"
          />
        </div>
      </div>
    </div>
  );
};

export default Branches;
