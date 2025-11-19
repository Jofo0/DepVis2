import { Trash } from "lucide-react";
import {
  useDeleteProjectMutation,
  useGetProjectBranchesQuery,
  useGetProjectQuery,
} from "../../store/api/projectsApi";
import { useEffect, useMemo, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { type Branch } from "@/types/branches";
import Processing from "./Processing";
import { Card, CardContent, CardHeader } from "@/components/ui/card";

const ProjectDetailPage = () => {
  const { id } = useParams<{ id: string }>();
  const { data: project, isLoading } = useGetProjectQuery(id!);
  const [removeProject, { isLoading: isRemoving }] = useDeleteProjectMutation();
  const { data: branches } = useGetProjectBranchesQuery(id!);

  const preferredDefault = useMemo(() => {
    if (!branches || branches.length === 0) return undefined;
    return branches[0];
  }, [branches]);

  const [selectedBranch, setSelectedBranch] = useState<Branch>();

  useEffect(() => {
    if (!branches || branches.length === 0) return;
    if (!selectedBranch || !branches.includes(selectedBranch)) {
      setSelectedBranch(preferredDefault ?? branches[0]);
    }
  }, [branches, preferredDefault, selectedBranch]);

  const navigate = useNavigate();
  const handleRemoveProject = async () => {
    if (project) {
      await removeProject(project.id);
      navigate("/");
    }
  };

  if (isLoading) return <p className="p-4 text-subtle">Loading...</p>;
  if (!project) return <p className="p-4 text-subtle">Project not found</p>;

  return (
    <div className="space-y-6">
      <Card>
        <CardHeader>Project Information</CardHeader>
        <CardContent className="flex flex-col">
          <p className="flex flex-row gap-1">
            <p className="font-bold">Project:</p>
            <p>{project.name}</p>
          </p>
          <p className="flex flex-row gap-1">
            <p className="font-bold"> GitHubLink:</p>
            <a className="text-gray-700" href={project.projectLink}>
              {project.projectLink}
            </a>
          </p>
        </CardContent>
      </Card>
      <Processing />
      <button
        onClick={handleRemoveProject}
        className="flex flex-row items-center gap-2 px-4 py-2 text-sm font-medium text-center text-white transition bg-red-700 rounded-lg hover:bg-red-700/60"
      >
        {isRemoving ? (
          "Removing..."
        ) : (
          <>
            Remove
            <Trash />
          </>
        )}
      </button>
    </div>
  );
};

export default ProjectDetailPage;
