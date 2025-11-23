import { DataTable } from "@/components/table/DataTable";
import { useGetBranchColumns } from "@/utils/columns/useGetBranchColumns";
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
import { toODataOrderBy } from "@/utils/odataHelper";

const Branches = () => {
  const projectId = useGetProjectId();
  const [sorting, setSorting] = useState<SortingState>([]);
  const columns = useGetBranchColumns();

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
    <div className="flex flex-col gap-3 w-full h-full">
      <div className="flex flex-row gap-10 w-full h-full justify-evenly">
        <div className="h-max-full w-2/3">
          <DataTable
            isLoading={isLoading}
            className="min-h-[calc(87vh)] max-h-[calc(87vh)]"
            table={table}
          />
        </div>
        <div className="flex flex-col gap-6 w-1/2 h-full">
          <XYChart
            className="min-h-[calc(42vh)] max-h-[calc(42vh)]"
            data={data}
            xKey="name"
            yKey="packageCount"
            yLabel="Packages"
          />

          <XYChart
            data={data}
            className="min-h-[calc(42vh)] max-h-[calc(42vh)]"
            xKey="name"
            yKey="vulnerabilityCount"
            yLabel="Vulnerabilities"
            color="#d12c2c"
          />
        </div>
      </div>
    </div>
  );
};

export default Branches;
