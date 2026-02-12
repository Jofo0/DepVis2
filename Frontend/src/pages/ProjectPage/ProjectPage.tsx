import {
  useDeleteProjectMutation,
  useGetProjectQuery,
} from "../../store/api/projectsApi";
import { useNavigate, useParams } from "react-router-dom";
import Processing from "./Processing";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
} from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Trash, ExternalLink, Cog } from "lucide-react";
import { useGetProjectBranchesQuery } from "@/store/api/branchesApi";
import InfoTab from "./ProjectInformation/InfoTab";
import InfoRow from "./ProjectInformation/InfoRow";
import { ChartLoader } from "@/components/chart/PieCustomChart";
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
} from "@/components/ui/dialog";
import { useState } from "react";
import { DialogDescription } from "@radix-ui/react-dialog";

const ProjectDetailPage = () => {
  const { id } = useParams<{ id: string }>();
  const { data: project, isLoading } = useGetProjectQuery(id!);
  const [removeProject, { isLoading: isRemoving }] = useDeleteProjectMutation();
  const { data } = useGetProjectBranchesQuery(id!);
  const [isDialogOpen, setIsDialogOpen] = useState(false);

  const branches = data?.items;

  const navigate = useNavigate();

  const handleRemoveProject = async () => {
    if (project) {
      await removeProject(project.id);
      navigate("/");
    }
  };

  if (isLoading)
    return (
      <div className="w-full h-full">
        <ChartLoader />
      </div>
    );

  if (!project) {
    navigate("/");
    return;
  }

  const mostVulnerable = branches?.reduce((max, b) => {
    return b.vulnerabilityCount > max.vulnerabilityCount ? b : max;
  });

  const mostPackages = branches?.reduce((max, b) => {
    return b.packageCount > max.packageCount ? b : max;
  });

  return (
    <div className="space-y-6">
      <Card>
        <CardHeader className="pb-3">
          <div className="flex items-start justify-between gap-4">
            <div className="min-w-0">
              <div className="text-lg font-semibold leading-tight">
                {project.name}
              </div>
              <CardDescription>Project Information</CardDescription>
            </div>

            <div className="flex items-center gap-2">
              <Button
                onClick={() => navigate(`edit`)}
                variant="secondary"
                size="sm"
              >
                {"Edit"}
                <Cog className="ml-2 h-4 w-4" />
              </Button>
              <Button
                onClick={() => setIsDialogOpen(true)}
                variant="destructive"
                size="sm"
                disabled={isRemoving}
              >
                {isRemoving ? "Removing..." : "Remove"}
                <Trash className="ml-2 h-4 w-4" />
              </Button>
            </div>
          </div>
        </CardHeader>

        <CardContent className="space-y-4">
          <div className="space-y-3">
            <InfoRow label="Project name">
              <span>{project.name}</span>
            </InfoRow>

            <InfoRow label="GitHub link">
              <a
                href={project.projectLink}
                target="_blank"
                rel="noreferrer"
                className="inline-flex items-center gap-1 text-primary underline-offset-4 hover:underline"
              >
                {project.projectLink}
                <ExternalLink className="h-4 w-4 opacity-70" />
              </a>
            </InfoRow>
          </div>

          <div className="h-px w-full bg-border" />
          <div className="grid grid-cols-2 sm:grid-cols-4 gap-3 w-full">
            <InfoTab
              title="Branches"
              info={branches?.filter((b) => !b.isTag)?.length ?? 0}
            />
            <InfoTab
              title="Tags"
              info={branches?.filter((b) => b.isTag)?.length ?? 0}
            />
            <InfoTab
              title="Branch/Tag with Most Packages"
              info={
                mostPackages
                  ? `${mostPackages.name} with ${mostPackages.packageCount} packages`
                  : "None"
              }
            />
            <InfoTab
              title="Most Vulnerable Branch/Tag"
              info={
                mostVulnerable
                  ? `${mostVulnerable.name} with ${mostVulnerable.vulnerabilityCount} vulnerabilities`
                  : "None"
              }
            />
          </div>
        </CardContent>
      </Card>

      <Processing />
      <Dialog open={isDialogOpen}>
        <DialogContent>
          <DialogHeader>Confirm Deletion</DialogHeader>
          <DialogDescription>
            Are you sure you want to delete this project? This action cannot be
            undone.
          </DialogDescription>

          <DialogFooter>
            <Button variant="outline" onClick={() => setIsDialogOpen(false)}>
              Cancel
            </Button>
            <Button variant="destructive" onClick={() => handleRemoveProject()}>
              Confirm
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
};

export default ProjectDetailPage;
