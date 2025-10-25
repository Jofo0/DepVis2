import { DataTable } from "@/components/table/DataTable";
import { useGetProjectBranchesDetailedQuery } from "@/store/api/projectsApi";
import { columns } from "@/utils/columns/branchColumns";
import { useGetProjectId } from "@/utils/hooks/useGetProjectId";

const Branches = () => {
  const projectId = useGetProjectId();
  const { data = [] } = useGetProjectBranchesDetailedQuery(projectId);

  return (
    <div className="container mx-auto py-10">
      <DataTable columns={columns} data={data} />
    </div>
  );
};

export default Branches;
