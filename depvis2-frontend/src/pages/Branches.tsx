import { BranchVulnChart } from "@/components/chart/BranchVulnChart";
import { BranchPackageChart } from "@/components/chart/BranchPackageChart";
import { DataTable } from "@/components/table/DataTable";
import { useGetProjectBranchesDetailedQuery } from "@/store/api/projectsApi";
import { columns } from "@/utils/columns/branchColumns";
import { useGetProjectId } from "@/utils/hooks/useGetProjectId";

const Branches = () => {
  const projectId = useGetProjectId();
  const { data = [] } = useGetProjectBranchesDetailedQuery(projectId);

  return (
    <div className="container py-10 flex flex-row gap-10">
      <DataTable columns={columns} data={data} className="w-3/4" />
      <div className="flex flex-col gap-4 w-1/2">
        <BranchPackageChart />
        <BranchVulnChart />
      </div>
    </div>
  );
};

export default Branches;
