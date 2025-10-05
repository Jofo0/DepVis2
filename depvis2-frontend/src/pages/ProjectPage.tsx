import { Trash } from "lucide-react";
import {
  useDeleteProjectMutation,
  useGetProjectBranchesQuery,
  useGetProjectQuery,
} from "../store/api/projectsApi";
import { useEffect, useMemo, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import SimpleGraph from "../components/graph/SimpleGraph";
import { ProjectStats } from "../components/project/ProjectStats";
import type { ProjectBranchDto } from "../types/projects";

const ProjectDetailPage = () => {
  const { id } = useParams<{ id: string }>();
  const { data: project, isLoading } = useGetProjectQuery(id!);
  const [removeProject, { isLoading: isRemoving }] = useDeleteProjectMutation();
  const { data: branches, isLoading: branchesLoading } =
    useGetProjectBranchesQuery(id!);

  const preferredDefault = useMemo(() => {
    if (!branches || branches.length === 0) return undefined;
    return branches[0];
  }, [branches]);

  const [selectedBranch, setSelectedBranch] = useState<ProjectBranchDto>();

  useEffect(() => {
    if (!branches || branches.length === 0) return;
    if (!selectedBranch || !branches.includes(selectedBranch)) {
      setSelectedBranch(preferredDefault ?? branches[0]);
    }
  }, [branches, preferredDefault, selectedBranch]);

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

      {!branchesLoading && selectedBranch && (
        <ProjectStats branch={selectedBranch} />
      )}

      <div className="space-y-2">
        <label htmlFor="branch" className="text-sm font-medium text-text">
          Branch
        </label>
        <select
          id="branch"
          className="w-full px-3 py-2 text-sm transition border rounded-xl bg-surface border-border focus:outline-none focus:ring-2 focus:ring-accent"
          value={selectedBranch?.id}
          onChange={(e) =>
            setSelectedBranch(branches?.find((x) => x.id == e.target.value))
          }
          disabled={branchesLoading || !branches || branches.length === 0}
        >
          {branchesLoading && <option>Loading branches…</option>}
          {!branchesLoading && (!branches || branches.length === 0) && (
            <option>No branches found</option>
          )}
          {(branches ?? []).map((branch) => (
            <option key={branch.id} value={branch.id}>
              {branch.name}
            </option>
          ))}
        </select>
      </div>

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
          onClick={handleRemoveProject}
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

      {selectedBranch && <SimpleGraph branch={selectedBranch} />}
    </div>
  );
};

export default ProjectDetailPage;
