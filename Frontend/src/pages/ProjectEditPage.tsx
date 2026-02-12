import { useGetProjectId } from "@/utils/hooks/useGetProjectId";
import ProjectEditForm from "./ProjectEditForm";

const ProjectEditPage = () => {
  const projectId = useGetProjectId();
  return (
    <div className="self-center w-full max-w-2xl">
      <ProjectEditForm projectId={projectId} />
    </div>
  );
};

export default ProjectEditPage;
