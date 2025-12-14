import BranchSelector from "@/components/BranchSelector";
import { PieCustomChart } from "@/components/chart/PieCustomChart";
import { DataTable } from "@/components/table/DataTable";
import { useLazyGetPackagesQuery } from "@/store/api/projectsApi";
import { buildPackagesOdata } from "@/utils/buildPackagesOdata";
import { useGetPackagesColumns } from "@/utils/columns/useGetPackagesColumns";
import { useBranch } from "@/utils/hooks/BranchProvider";
import { toODataOrderBy } from "@/utils/odataHelper";
import {
  useReactTable,
  getSortedRowModel,
  getCoreRowModel,
  getPaginationRowModel,
  type SortingState,
} from "@tanstack/react-table";
import { useEffect, useState } from "react";

const Packages = () => {
  const { branch, isLoading: isLoadingBranch } = useBranch();
  const columns = useGetPackagesColumns();
  const [sorting, setSorting] = useState<SortingState>([]);
  const [ecosystemFilter, setEcosystemFilter] = useState("");
  const [vulnerabilityFilter, setVulnerabilityFilter] = useState("");
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
    getPaginationRowModel: getPaginationRowModel(),
    manualSorting: true,
    getCoreRowModel: getCoreRowModel(),
  });

  useEffect(() => {
    if (!branch) return;

    const odata = buildPackagesOdata({
      ecosystem: ecosystemFilter || null,
      vulnerability: vulnerabilityFilter || null,
    });

    const sortOdata = toODataOrderBy(sorting);

    const fullOdata = [odata, sortOdata].filter(Boolean).join("&");
    fetchPackages(
      {
        id: branch.id,
        odata: fullOdata,
      },
      true
    );
  }, [branch, ecosystemFilter, vulnerabilityFilter, fetchPackages, sorting]);

  const onEcosystemClick = (name: string) => {
    setEcosystemFilter((prev) => (prev === name ? "" : name));
  };

  const onVulnerabilityClick = (name: string) => {
    setVulnerabilityFilter((prev) => (prev === name ? "" : name));
  };
  return (
    <div className="flex flex-col gap-3 w-full h-full">
      <div className="flex flex-col w-full h-full justify-evenly gap-2">
        <BranchSelector />
        {!isLoadingBranch && branch && (
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
                isLoading={isLoading}
                filteredBy={ecosystemFilter}
                onSliceClick={onEcosystemClick}
              />

              <PieCustomChart
                title="Vulnerabilities"
                onSliceClick={onVulnerabilityClick}
                isLoading={isLoading}
                className="min-h-[calc(42vh)] max-h-[calc(42vh)]"
                filteredBy={vulnerabilityFilter}
                pies={data?.vulnerabilities ?? []}
              />
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default Packages;
