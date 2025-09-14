import { Link } from "react-router-dom";
import { Moon, Sun } from "lucide-react";
import { useTheme } from "../theme/ThemeProvider";

const Layout = ({ children }: { children: React.ReactNode }) => {
  const { darkMode, toggle } = useTheme();

  return (
    <div className="flex flex-col w-screen min-h-screen bg-background text-text">
      <nav className="border-b border-border bg-surface">
        <div className="flex items-center justify-between max-w-5xl px-6 py-4 mx-auto">
          <Link to="/projects" className="text-lg font-semibold text-accent">
            DepVis Projects
          </Link>

          <div className="flex items-center space-x-6">
            <Link
              to="/projects"
              className="text-sm font-medium transition text-subtle hover:text-accent"
            >
              Projects
            </Link>
            <Link
              to="/projects/new"
              className="text-sm font-medium transition text-subtle hover:text-accent"
            >
              New Project
            </Link>

            <button
              onClick={toggle}
              aria-label="Toggle theme"
              className="p-2 transition border rounded-lg border-border bg-background hover:bg-background/60"
            >
              {darkMode ? (
                <Sun className="w-4 h-4 text-accent" />
              ) : (
                <Moon className="w-4 h-4 text-accent" />
              )}
            </button>
          </div>
        </div>
      </nav>

      <main className="flex flex-col justify-start flex-1 w-full max-w-5xl px-6 py-8 mx-auto">
        {children}
      </main>
    </div>
  );
};

export default Layout;
