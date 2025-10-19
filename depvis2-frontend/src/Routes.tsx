import { Routes as RoutesReactRouter, Route, Outlet } from "react-router-dom";
import ProjectCreatePage from "./pages/ProjectCreatePage";
import ProjectDetailPage from "./pages/ProjectPage";
import Layout from "./pages/Layout";
import SelectProjectPage from "./pages/SelectProjectPage";
import Branches from "./pages/Branches";
import Graph from "./pages/Graph";

const Routes = () => {
  return (
    <RoutesReactRouter>
      <Route
        element={
          <div className="self-center w-dvw h-dvh flex justify-center items-center">
            <Outlet />
          </div>
        }
      >
        <Route path="/" element={<SelectProjectPage />} />
        <Route path="/new" element={<ProjectCreatePage />} />
      </Route>
      <Route element={<Layout />}>
        <Route path="/:id" element={<ProjectDetailPage />} />
        <Route path="/:id/branches" element={<Branches />} />
        <Route path="/:id/packages" element={<ProjectDetailPage />} />
        <Route path="/:id/vulnerabilities" element={<ProjectDetailPage />} />
        <Route path="/:id/graph" element={<Graph />} />
      </Route>
    </RoutesReactRouter>
  );
};

export default Routes;
