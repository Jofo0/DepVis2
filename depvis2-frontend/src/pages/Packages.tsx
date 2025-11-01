import BranchSelector from "@/components/BranchSelector";
import { PieCustomChart } from "@/components/chart/PieCustomChart";
import { DataTable } from "@/components/table/DataTable";
import { useLazyGetPackagesQuery } from "@/store/api/projectsApi";
import { useGetPackagesColumns } from "@/utils/columns/useGetPackagesColumns";
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
  const columns = useGetPackagesColumns();

  const [fetchPackages, { data, isFetching: isLoading }] =
    useLazyGetPackagesQuery();

  const table = useReactTable({
    data: data?.packageItems ?? [],
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
      fetchPackages(
        {
          id: branch.id,
          odata: toODataOrderBy(sorting),
        },
        true
      );
    }
  }, [branch]);

  return (
    <div className="flex flex-col gap-3 w-full h-full py-4">
      <div className="flex flex-col  w-full h-full justify-evenly">
        <BranchSelector />
        <div className="flex flex-row gap-10 w-full h-full justify-evenly">
          <div className="h-max-full w-1/2">
            <DataTable
              isLoading={isLoading}
              className="min-h-[calc(100vh-9rem)] max-h-[calc(100vh-9rem)]"
              table={table}
            />
          </div>
          <div className="flex flex-col gap-6 w-1/2 h-full">
            <PieCustomChart
              title="Ecosystems"
              className="min-h-[calc(42vh)] max-h-[calc(42vh)]"
              pies={data?.ecoSystems ?? []}
            />

            <PieCustomChart
              title="Vulnerabilities"
              className="min-h-[calc(42vh)] max-h-[calc(42vh)]"
              pies={data?.vulnerabilities ?? []}
            />
          </div>
        </div>
      </div>
    </div>
  );
};

export default Packages;
