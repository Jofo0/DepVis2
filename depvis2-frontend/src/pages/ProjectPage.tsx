import { Trash } from "lucide-react";
import {
  useDeleteProjectMutation,
  useGetProjectQuery,
} from "../services/projectsApi";
import { useNavigate, useParams } from "react-router-dom";

const ProjectDetailPage = () => {
  const { id } = useParams<{ id: string }>();
  const { data: project, isLoading } = useGetProjectQuery(id!);
  const [removeProject, { isLoading: isRemoving }] = useDeleteProjectMutation();
  const navigate = useNavigate();

  const handleRemoveProject = async () => {
    if (project) {
      await removeProject(project.id);
      navigate("/projects");
    }
  };

  if (isLoading) return <p className="p-4 text-subtle">Loading...</p>;
  if (!project) return <p className="p-4 text-subtle">Project not found</p>;

  return (
    <div className="space-y-6">
      <h2 className="text-2xl font-medium tracking-tight text-text">
        {project.name}
      </h2>

      <div className="p-6 space-y-3 border bg-surface border-border rounded-2xl">
        <p className="text-sm">
          <span className="font-medium text-text">Type:</span>{" "}
          <span className="text-subtle">{project.projectType}</span>
        </p>

        <p className="text-sm">
          <span className="font-medium text-text">Status:</span>{" "}
          <span className="text-subtle">{project.processStatus}</span>
        </p>

        <p className="text-sm">
          <span className="font-medium text-text">Step:</span>{" "}
          <span className="text-subtle">{project.processStep}</span>
        </p>

        {project.projectLink && (
          <p className="text-sm">
            <span className="font-medium text-text">Link:</span>{" "}
            <a
              href={project.projectLink}
              className="text-accent hover:underline"
              target="_blank"
              rel="noreferrer"
            >
              {project.projectLink}
            </a>
          </p>
        )}

        <button
          onClick={() => handleRemoveProject()}
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
    </div>
  );
};

export default ProjectDetailPage;
