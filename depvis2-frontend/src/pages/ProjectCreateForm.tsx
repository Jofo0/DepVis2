import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { useCreateProjectMutation } from "../store/api/projectsApi";
import type { CreateProjectDto } from "../types/projects";
import { useNavigate } from "react-router-dom";
import { useLazyGetGitInformationQuery } from "../store/api/gitApi";
import { useState } from "react";

const ProjectSchema = z.object({
  name: z.string().min(1, "Name is required"),
  projectType: z.enum(["GitHub"]),
  projectLink: z
    .string()
    .url("Must be a valid URL")
    .optional()
    .or(z.literal("")),
  selectedBranches: z.array(z.string()).optional(),
  selectedTags: z.array(z.string()).optional(),
});

type FormValues = z.infer<typeof ProjectSchema>;

const ProjectCreateForm = () => {
  const [createProject, { isLoading }] = useCreateProjectMutation();
  const [isGitAvailable, setIsGitAvailable] = useState(false);
  const [
    retrieveGitInfo,
    { isLoading: gitLoading, data: gitData, error: gitError },
  ] = useLazyGetGitInformationQuery();

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
    watch,
  } = useForm<FormValues>({
    resolver: zodResolver(ProjectSchema),
    defaultValues: {
      name: "",
      projectType: "GitHub",
      projectLink: "",
      selectedBranches: [],
      selectedTags: [],
    },
  });

  const projectLink = watch("projectLink");
  const navigate = useNavigate();

  const onSubmit = async (values: FormValues) => {
    try {
      const payload: CreateProjectDto = {
        name: values.name,
        projectType: values.projectType,
        projectLink: values.projectLink || "",
        branches: values.selectedBranches ?? [],
        tags: values.selectedTags ?? [],
      };

      const result = await createProject(payload).unwrap();
      navigate(`/projects/${result.id}`);
    } catch (err) {
      console.error("Failed to create project", err);
    }
  };

  const handleFetchGit = async () => {
    const url = projectLink;
    if (!url) return;

    try {
      const res = await retrieveGitInfo(encodeURIComponent(url));
      setIsGitAvailable(!res.error);
    } catch (e) {
      setIsGitAvailable(false);
      console.error("Failed to fetch git info", e);
    }
  };

  return (
    <form
      onSubmit={handleSubmit(onSubmit)}
      className="w-full p-8 space-y-6 border bg-background text-text border-border rounded-2xl"
    >
      <h3 className="text-xl font-medium tracking-tight">Create Project</h3>

      {/* Name */}
      <div className="space-y-1">
        <label className="block text-sm font-medium">Name</label>
        <input
          {...register("name")}
          className={`w-full rounded-xl border px-4 py-2 text-sm bg-surface focus:outline-hidden focus:ring-2 focus:ring-accent transition ${
            errors.name ? "border-red-400" : "border-border"
          }`}
          placeholder="Project Name"
        />
        {errors.name && (
          <p className="text-xs text-red-500">{errors.name.message}</p>
        )}
      </div>

      {/* Project type */}
      <div className="space-y-1">
        <label className="block text-sm font-medium">Project type</label>
        <select
          {...register("projectType")}
          className="w-full px-4 py-2 text-sm transition border rounded-xl bg-surface border-border focus:outline-hidden focus:ring-2 focus:ring-accent"
        >
          <option value="GitHub">GitHub Link</option>
        </select>
        {errors.projectType && (
          <p className="text-xs text-red-500">{errors.projectType.message}</p>
        )}
      </div>

      {/* GitHub Link + Fetch button */}
      <div className="space-y-1">
        <label className="block text-sm font-medium">GitHub Link</label>
        <div className="flex gap-2">
          <input
            {...register("projectLink")}
            className={`flex-1 rounded-xl border px-4 py-2 text-sm bg-surface focus:outline-hidden focus:ring-2 focus:ring-accent transition ${
              errors.projectLink ? "border-red-400" : "border-border"
            }`}
            placeholder="https://github.com/owner/repo"
          />
          <button
            type="button"
            onClick={handleFetchGit}
            disabled={!projectLink || !!errors.projectLink || gitLoading}
            className="px-3 py-2 text-sm transition border rounded-xl border-border bg-surface hover:bg-surface/70 disabled:opacity-60"
            title={
              !projectLink
                ? "Enter a GitHub URL first"
                : "Fetch branches & tags"
            }
          >
            {gitLoading ? "Fetching..." : "Fetch Git info"}
          </button>
        </div>
        {errors.projectLink && (
          <p className="text-xs text-red-500">{errors.projectLink.message}</p>
        )}
        {gitError && (
          <p className="text-xs text-red-500">
            Couldn’t fetch repository info. Check the URL or your credentials.
          </p>
        )}
      </div>

      {/* Branches / Tags multi-selects (only when data is present) */}
      {gitData && (
        <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
          <div className="space-y-1">
            <label className="block text-sm font-medium">Branches</label>
            <select
              multiple
              {...register("selectedBranches")}
              className="w-full px-3 py-2 text-sm transition border min-h-36 rounded-xl bg-surface border-border focus:outline-hidden focus:ring-2 focus:ring-accent"
            >
              {(gitData?.branches ?? []).map((b: string) => (
                <option key={b} value={b}>
                  {b}
                </option>
              ))}
            </select>
            <p className="text-xs text-muted-foreground">
              Hold Ctrl / Cmd to select multiple.
            </p>
          </div>

          <div className="space-y-1">
            <label className="block text-sm font-medium">Tags</label>
            <select
              multiple
              {...register("selectedTags")}
              className="w-full px-3 py-2 text-sm transition border min-h-36 rounded-xl bg-surface border-border focus:outline-hidden focus:ring-2 focus:ring-accent"
            >
              {(gitData?.tags ?? []).map((t: string) => (
                <option key={t} value={t}>
                  {t}
                </option>
              ))}
            </select>
            <p className="text-xs text-muted-foreground">
              Hold Ctrl / Cmd to select multiple.
            </p>
          </div>
        </div>
      )}

      {/* Actions */}
      <div className="flex items-center justify-end pt-4 space-x-3">
        <button
          type="button"
          onClick={() => reset()}
          className="px-4 py-2 text-sm transition border rounded-xl border-border hover:bg-surface"
        >
          Reset
        </button>

        <button
          type="submit"
          disabled={isLoading}
          className="px-4 py-2 text-sm text-white transition rounded-xl bg-accent hover:opacity-90 disabled:opacity-60"
        >
          {isLoading ? "Creating..." : "Create Project"}
        </button>
      </div>
    </form>
  );
};

export default ProjectCreateForm;
