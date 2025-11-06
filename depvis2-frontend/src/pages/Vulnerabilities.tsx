import BranchSelector from "@/components/BranchSelector";
import { DataTable } from "@/components/table/DataTable";
import { Card } from "@/components/ui/card";
import { useLazyGetVulnerabilitiesQuery } from "@/store/api/projectsApi";
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
  const [vulnerabilityFilter, setVulnerabilityFilter] = useState("");
  const [fetchVulnerabilities, { data, isFetching: isLoading }] =
    useLazyGetVulnerabilitiesQuery();

  const table = useReactTable({
    data: [],
    columns,
    enableRowSelection: true,
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
      },
      true
    );
  }, [branch, vulnerabilityFilter, fetchVulnerabilities]);

  const onVulnerabilityClick = (name: string) => {
    setVulnerabilityFilter((prev) => (prev === name ? "" : name));
  };
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
          <div className="flex flex-col gap-6 w-1/2 h-full items-center justify-center">
            <Card className="h-1/6 w-1/3 flex flex-col  items-center justify-center border-dashed border-3 text-gray-600">
              Select a Vulnerability to see more details
            </Card>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Vulnerabilities;
