import { useGetProjectsQuery } from "../services/projectsApi";
import { Link } from "react-router-dom";

const ProjectsListPage = () => {
  const { data: projects, isLoading } = useGetProjectsQuery();

  if (isLoading) return <p className="p-4 text-subtle">Loading...</p>;

  return (
    <div className="space-y-6">
      <h2 className="text-2xl font-medium tracking-tight text-text">
        Projects
      </h2>

      <ul className="overflow-hidden border divide-y border-border rounded-2xl divide-border bg-surface">
        {projects?.map((p) => (
          <li key={p.id} className="transition hover:bg-background/60">
            <Link to={`/projects/${p.id}`} className="block p-4">
              <div className="font-medium text-text">{p.name}</div>
              <div className="mt-1 text-xs text-subtle">
                {p.projectType} · {p.processStatus}
              </div>
            </Link>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default ProjectsListPage;
