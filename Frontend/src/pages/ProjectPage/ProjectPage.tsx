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
import { Button } from "@/components/ui/button";
import { Trash } from "lucide-react";

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
          <Button
            onClick={handleRemoveProject}
            variant={"destructive"}
            className="w-24"
          >
            {isRemoving ? (
              "Removing..."
            ) : (
              <>
                Remove
                <Trash />
              </>
            )}
          </Button>
        </CardContent>
      </Card>
      <Processing />
    </div>
  );
};

export default ProjectDetailPage;
