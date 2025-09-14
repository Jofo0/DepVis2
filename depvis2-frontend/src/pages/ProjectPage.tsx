import { useGetProjectQuery } from "../services/projectsApi";
import { useParams } from "react-router-dom";
const ProjectDetailPage = () => {
  const { id } = useParams<{ id: string }>();
  const { data: project, isLoading } = useGetProjectQuery(id!);

  if (isLoading) return <p className="p-4">Loading...</p>;
  if (!project) return <p className="p-4">Project not found</p>;

  return (
    <div className="space-y-4">
      <h2 className="text-xl font-semibold">{project.name}</h2>
      <div className="text-sm text-slate-600">
        <p>
          <span className="font-medium">Type:</span> {project.projectType}
        </p>
        <p>
          <span className="font-medium">Status:</span> {project.processStatus}
        </p>
        <p>
          <span className="font-medium">Step:</span> {project.processStep}
        </p>
        {project.projectLink && (
          <p>
            <span className="font-medium">Link:</span>{" "}
            <a
              href={project.projectLink}
              className="text-indigo-600 hover:underline"
              target="_blank"
              rel="noreferrer"
            >
              {project.projectLink}
            </a>
          </p>
        )}
      </div>
    </div>
  );
};

export default ProjectDetailPage;
