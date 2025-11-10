import { Trash } from "lucide-react";
import {
  useDeleteProjectMutation,
  useGetProjectBranchesQuery,
  useGetProjectQuery,
} from "../store/api/projectsApi";
import { useEffect, useMemo, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { Card, CardContent, CardHeader } from "@/components/ui/card";
import { ProcessStep, type Branch } from "@/types/branches";
import ProcessingCard from "@/components/cards/ProcessingCard";

const ProjectDetailPage = () => {
  const { id } = useParams<{ id: string }>();
  const { data: project, isLoading } = useGetProjectQuery(id!);
  const [removeProject, { isLoading: isRemoving }] = useDeleteProjectMutation();
  const { data: branches, isLoading: branchesLoading } =
    useGetProjectBranchesQuery(id!);

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
      <Card className="border-0 shadow-none">
        <CardHeader>Processing</CardHeader>

        <CardContent className="flex flex-row justify-around w-full gap-4 items-center">
          <ProcessingCard
            branches={
              branches?.filter((b) => b.processStep === ProcessStep.Created) ??
              []
            }
            header="Preparing for Processing"
            description="Branch/Tag/Commit is ready to be processed"
          />
          <ProcessingCard
            branches={
              branches?.filter(
                (b) => b.processStep === ProcessStep.SbomCreation
              ) ?? []
            }
            header="Creating SBOM"
            description="Branch/Tag/Commit is being processed and the SBOM is being created"
          />

          <ProcessingCard
            branches={
              branches?.filter(
                (b) => b.processStep === ProcessStep.SbomIngest
              ) ?? []
            }
            header="Processing SBOM"
            description="The SBOM is being processed and the reports are being generated"
          />

          <ProcessingCard
            branches={
              branches?.filter(
                (b) => b.processStep === ProcessStep.Processed
              ) ?? []
            }
            header="Finished Processing"
            description="Branch/Tag/Commit has finished processing and reports are ready"
          />
        </CardContent>
      </Card>
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
