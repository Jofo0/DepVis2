import BranchSelector from "@/components/BranchSelector";
import { PieCustomChart } from "@/components/chart/PieCustomChart";
import SimpleGraph from "@/components/graph/SimpleGraph";
import { DataTable } from "@/components/table/DataTable";
import { Card, CardContent, CardHeader } from "@/components/ui/card";
import {
  useLazyGetVulnerabilitiesQuery,
  useLazyGetVulnerabilityQuery,
} from "@/store/api/projectsApi";
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
  const [fetchVulnerability, { data: vulnData, isFetching: isLoadingVuln }] =
    useLazyGetVulnerabilityQuery();

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

  useEffect(() => {
    if (selectedVulnerability) {
      fetchVulnerability(selectedVulnerability.vulnerabilityId);
    }
  }, [selectedVulnerability, fetchVulnerability]);

  const onRiskClick = (name: string) => {
    setRiskFilter((prev) => (prev === name ? "" : name));
  };

  return (
    <div className="flex flex-col gap-3 w-full h-full py-4">
      <div className="flex flex-col  w-full h-full justify-evenly">
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
              <Card
                className={`overflow-hidden border-3 self-center ${
                  selectedVulnerability
                    ? "h-full w-full max-h-full max-w-full"
                    : "h-1/6 w-1/3 m-3.5 border-dashed "
                } transition-all duration-500 ease-in-out`}
              >
                {selectedVulnerability ? (
                  <>
                    {isLoadingVuln ? (
                      <p>Loading vulnerability details...</p>
                    ) : (
                      vulnData && (
                        <>
                          <CardHeader>Vulnerability: {vulnData.id}</CardHeader>
                          <CardContent className="text-lg">
                            <p>
                              <strong>Severity: </strong>
                              {vulnData.severity}
                            </p>
                            <p>
                              <strong>Description: </strong>
                              {vulnData.description}
                            </p>
                            <p>
                              <strong>Recommendation: </strong>
                              {vulnData.recommendation}
                            </p>
                          </CardContent>
                          <div className="max-h-4/6 max-w-full m-4">
                            <SimpleGraph
                              lr
                              branch={branch!}
                              packageId={selectedVulnerability.packageId}
                            />
                          </div>
                        </>
                      )
                    )}
                  </>
                ) : (
                  <div className="flex items-center justify-center h-full">
                    <p>Select a Vulnerability to see more details</p>
                  </div>
                )}
              </Card>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Vulnerabilities;
