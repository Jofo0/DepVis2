import BranchSelector from "@/components/BranchSelector";
import { PieCustomChart } from "@/components/chart/PieCustomChart";
import { DataTable } from "@/components/table/DataTable";
import { useLazyGetPackagesQuery } from "@/store/api/projectsApi";
import { buildPackagesOdata } from "@/utils/buildPackagesOdata";
import { useGetPackagesColumns } from "@/utils/columns/useGetPackagesColumns";
import { useBranch } from "@/utils/hooks/BranchProvider";
import {
  useReactTable,
  getSortedRowModel,
  getCoreRowModel,
  getPaginationRowModel,
} from "@tanstack/react-table";
import { useEffect, useState } from "react";

const Packages = () => {
  const { branch } = useBranch();
  const columns = useGetPackagesColumns();
  const [ecosystemFilter, setEcosystemFilter] = useState("");
  const [vulnerabilityFilter, setVulnerabilityFilter] = useState("");
  const [fetchPackages, { data, isFetching: isLoading }] =
    useLazyGetPackagesQuery();

  const table = useReactTable({
    data: data?.packageItems ?? [],
    columns,
    getSortedRowModel: getSortedRowModel(),
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

    fetchPackages(
      {
        id: branch.id,
        odata,
      },
      true
    );
  }, [branch, ecosystemFilter, vulnerabilityFilter, fetchPackages]);

  const onEcosystemClick = (name: string) => {
    setEcosystemFilter((prev) => (prev === name ? "" : name));
  };

  const onVulnerabilityClick = (name: string) => {
    setVulnerabilityFilter((prev) => (prev === name ? "" : name));
  };
  return (
    <div className="flex flex-col gap-3 w-full h-full py-4">
      <div className="flex flex-col w-full h-full justify-evenly gap-2">
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
              isLoading={isLoading}
              onSliceClick={onEcosystemClick}
            />

            <PieCustomChart
              title="Vulnerabilities"
              onSliceClick={onVulnerabilityClick}
              isLoading={isLoading}
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
