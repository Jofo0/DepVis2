import { Routes as RoutesReactRouter, Route } from "react-router-dom";
import ProjectCreatePage from "./pages/ProjectCreatePage";
import ProjectDetailPage from "./pages/ProjectPage";

const Routes = () => {
  return (
    <RoutesReactRouter>
      <Route path="/new" element={<ProjectCreatePage />} />
      <Route path="/:id" element={<ProjectDetailPage />} />
    </RoutesReactRouter>
  );
};

export default Routes;
