import { useGetProjectsQuery } from "../services/projectsApi";
import { Link } from "react-router-dom";

const ProjectsListPage = () => {
  const { data: projects, isLoading } = useGetProjectsQuery();

  if (isLoading) return <p className="p-4">Loading...</p>;

  return (
    <div className="space-y-4">
      <h2 className="text-xl font-semibold">Projects</h2>

      <ul className="border divide-y rounded-lg divide-slate-200 border-slate-200">
        {projects?.map((p) => (
          <li key={p.id} className="p-3 hover:bg-slate-50">
            <Link to={`/projects/${p.id}`} className="block">
              <div className="font-medium">{p.name}</div>
              <div className="text-xs text-slate-500">
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
