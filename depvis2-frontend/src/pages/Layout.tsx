import { Link } from "react-router-dom";

const Layout = ({ children }: { children: React.ReactNode }) => {
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
};

export default Layout;
