import { Routes as RoutesReactRouter, Route } from "react-router-dom";
import ProjectCreatePage from "./pages/ProjectCreatePage";
import ProjectsListPage from "./pages/ProjectListPage";
import ProjectDetailPage from "./pages/ProjectPage";

const Routes = () => {
  return (
    <RoutesReactRouter>
      <Route path="/projects" element={<ProjectsListPage />} />
      <Route path="/projects/new" element={<ProjectCreatePage />} />
      <Route path="/projects/:id" element={<ProjectDetailPage />} />
    </RoutesReactRouter>
  );
};

export default Routes;
