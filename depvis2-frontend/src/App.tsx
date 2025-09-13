import React from "react";
import {
  BrowserRouter as Router,
  Routes,
  Route,
  Link,
  useParams,
} from "react-router-dom";

import ProjectCreateForm from "./ProjectCreateForm";
import {
  useGetProjectsQuery,
  useGetProjectQuery,
} from "./services/projectsApi";

// Navbar layout
function Layout({ children }: { children: React.ReactNode }) {
  return (
    <div className="flex flex-col w-screen min-h-screen min-w-screen bg-slate-50 dark:bg-slate-900">
      <nav className="bg-white shadow-md dark:bg-slate-800">
        <div className="flex items-center justify-between max-w-5xl px-4 py-3 mx-auto">
          <Link
            to="/projects"
            className="text-lg font-semibold text-indigo-600"
          >
            DepVis Projects
          </Link>
          <div className="space-x-4">
            <Link
              to="/projects"
              className="text-sm font-medium text-slate-700 dark:text-slate-200 hover:text-indigo-600"
            >
              Projects
            </Link>
            <Link
              to="/projects/new"
              className="text-sm font-medium text-slate-700 dark:text-slate-200 hover:text-indigo-600"
            >
              New Project
            </Link>
          </div>
        </div>
      </nav>

      <main className="flex-1 w-full max-w-5xl p-6 mx-auto">{children}</main>
    </div>
  );
}

// List page
function ProjectsListPage() {
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
}

// Detail page
function ProjectDetailPage() {
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
}

// Create page
function ProjectCreatePage() {
  return (
    <div className="">
      <ProjectCreateForm
        onSuccess={(created) => {
          console.log("Created", created);
        }}
      />
    </div>
  );
}

// Root router
export default function App() {
  return (
    <Router>
      <Layout>
        <Routes>
          <Route path="/projects" element={<ProjectsListPage />} />
          <Route path="/projects/new" element={<ProjectCreatePage />} />
          <Route path="/projects/:id" element={<ProjectDetailPage />} />
        </Routes>
      </Layout>
    </Router>
  );
}
