import BranchSelector from "@/components/BranchSelector";
import { PieCustomChart } from "@/components/chart/PieCustomChart";
import { DataTable } from "@/components/table/DataTable";
import VulnerabilityCard from "@/components/VulnerabilityCard";
import { useLazyGetVulnerabilitiesQuery } from "@/store/api/projectsApi";
import type { VulnerabilitySmallDto } from "@/types/vulnerabilities";
import { buildOdata } from "@/utils/buildGeneralOdata";
import { useGetVulnerabilitiesColumns } from "@/utils/columns/useGetVulnerabilitiesColumns";
import { useBranch } from "@/utils/hooks/BranchProvider";
import {
  useReactTable,
  getSortedRowModel,
  getCoreRowModel,
  getPaginationRowModel,
} from "@tanstack/react-table";
import { useEffect, useState } from "react";

const Vulnerabilities = () => {
  const { branch } = useBranch();
  const columns = useGetVulnerabilitiesColumns();
  const [riskFilter, setRiskFilter] = useState("");
  const [fetchVulnerabilities, { data, isFetching: isLoading }] =
    useLazyGetVulnerabilitiesQuery();

  const [selectedVulnerability, setSelectedVulnerability] =
    useState<VulnerabilitySmallDto | null>(null);

  const table = useReactTable({
    data: data?.vulnerabilities || [],
    columns,
    getSortedRowModel: getSortedRowModel(),
    getPaginationRowModel: getPaginationRowModel(),
    manualSorting: true,
    getCoreRowModel: getCoreRowModel(),
  });

  useEffect(() => {
    if (!branch) return;

    fetchVulnerabilities(
      {
        id: branch.id,
        odata: buildOdata({
          severity: riskFilter || "",
        }),
      },
      true
    );
  }, [branch, riskFilter]);

  const onRiskClick = (name: string) => {
    setRiskFilter((prev) => (prev === name ? "" : name));
  };

  return (
    <div className="flex flex-col gap-3 w-full h-full py-4">
      <div className="flex flex-col w-full h-full justify-evenly gap-2">
        <BranchSelector />
        <div className="flex flex-row gap-10 w-full h-full justify-evenly">
          <div className="h-max-full w-1/2">
            <DataTable
              isLoading={isLoading}
              onClick={(row) => setSelectedVulnerability(row)}
              className="min-h-[calc(100vh-9rem)] max-h-[calc(100vh-9rem)]"
              table={table}
            />
          </div>
          <div className="flex flex-col gap-6 w-1/2 h-full ">
            <PieCustomChart
              title="Risk Severities"
              className="min-h-[calc(42vh)] max-h-[calc(42vh)]"
              pies={data?.risks ?? []}
              isLoading={isLoading}
              onSliceClick={onRiskClick}
            />
            <div
              className={`flex items-center justify-center self-center h-1/2 w-full`}
            >
              <VulnerabilityCard
                selectedVulnerability={selectedVulnerability}
              />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Vulnerabilities;
