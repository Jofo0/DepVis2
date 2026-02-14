import { useGetProjectId } from "@/utils/hooks/useGetProjectId";
import ProjectEditForm from "./ProjectEditForm";
import { Card, CardHeader } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { useState } from "react";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
} from "@/components/ui/dialog";
import { Trash } from "lucide-react";
import { useNavigate } from "react-router-dom";
import { useDeleteProjectMutation } from "@/store/api/projectsApi";

const ProjectEditPage = () => {
  const projectId = useGetProjectId();
  const [removeProject, { isLoading: isRemoving }] = useDeleteProjectMutation();
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const navigate = useNavigate();

  const handleRemoveProject = async () => {
    await removeProject(projectId);
    navigate("/");
  };

  return (
    <div className="self-center w-full h-full flex flex-col justify-center items-center gap-4">
      <ProjectEditForm projectId={projectId} className="w-1/2" />
      <Card className="w-1/4">
        <CardHeader>Remove Project</CardHeader>
        <Button
          className="w-1/3 self-end"
          onClick={() => setIsDialogOpen(true)}
          variant="destructive"
          size="sm"
          disabled={isRemoving}
        >
          Remove
          <Trash className="ml-2 h-4 w-4" />
        </Button>
      </Card>
      <Dialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
        <DialogContent>
          <DialogHeader>Confirm Deletion</DialogHeader>
          <DialogDescription>
            Are you sure you want to delete this project? This action cannot be
            undone.
          </DialogDescription>

          <DialogFooter>
            <Button
              variant="outline"
              disabled={isRemoving}
              onClick={() => setIsDialogOpen(false)}
            >
              Cancel
            </Button>
            <Button
              variant="destructive"
              disabled={isRemoving}
              onClick={() => handleRemoveProject()}
            >
              {isRemoving ? "Removing..." : "Confirm"}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
};

export default ProjectEditPage;
