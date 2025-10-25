import { BranchVulnChart } from "@/components/chart/BranchVulnChart";
import { BranchPackageChart } from "@/components/chart/BranchPackageChart";
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

  const { data = [] } = useGetProjectBranchesDetailedQuery({
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
            className="min-h-[calc(100vh-8.5rem)] max-h-[calc(100vh-8.5rem)]"
            table={table}
          />
        </div>
        <div className="flex flex-col gap-4 w-1/2 h-full">
          <BranchPackageChart data={data} />
          <BranchVulnChart data={data} />
        </div>
      </div>
    </div>
  );
};

export default Branches;
