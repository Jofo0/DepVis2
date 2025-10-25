import { BranchVulnChart } from "@/components/chart/BranchVulnChart";
import { BranchPackageChart } from "@/components/chart/BranchPackageChart";
import { DataTable } from "@/components/table/DataTable";
import { columns } from "@/utils/columns/branchColumns";
import { useGetProjectId } from "@/utils/hooks/useGetProjectId";
import { useGetProjectBranchesDetailedQuery } from "@/store/api/projectsApi";

const Branches = () => {
  const projectId = useGetProjectId();
  const { data = [] } = useGetProjectBranchesDetailedQuery(projectId);
  return (
    <div className="flex flex-col gap-3 w-full h-full py-8">
      <div className="flex flex-row gap-10 w-full h-full justify-evenly">
        <div className="h-max-full w-full">
          <DataTable
            columns={columns}
            data={data}
            className="min-h-[calc(100vh-8.5rem)] max-h-[calc(100vh-8.5rem)]"
          />
        </div>
        <div className="flex flex-col gap-4 w-1/2 h-full">
          <BranchPackageChart />
          <BranchVulnChart />
        </div>
      </div>
    </div>
  );
};

export default Branches;
