import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { useCreateProjectMutation } from "../services/projectsApi";
import type { CreateProjectDto } from "../types/projects";
import { useNavigate } from "react-router-dom";

// Validation schema
const ProjectSchema = z.object({
  name: z.string().min(1, "Name is required"),
  projectType: z.enum(["GitHub"]),
  projectLink: z
    .string()
    .url("Must be a valid URL")
    .optional()
    .or(z.literal("")),
});

type FormValues = z.infer<typeof ProjectSchema>;

const ProjectCreateForm = () => {
  const [createProject, { isLoading }] = useCreateProjectMutation();

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm<FormValues>({
    resolver: zodResolver(ProjectSchema),
    defaultValues: { name: "", projectType: "GitHub", projectLink: "" },
  });

  const navigate = useNavigate();
  const onSubmit = async (values: FormValues) => {
    try {
      const payload: CreateProjectDto = {
        name: values.name,
        projectType: values.projectType,
        projectLink: values.projectLink || "",
      };

      await createProject(payload).unwrap();
      navigate("/projects");
    } catch (err) {
      console.error("Failed to create project", err);
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
          className={`w-full rounded-xl border px-4 py-2 text-sm bg-surface focus:outline-none focus:ring-2 focus:ring-accent transition ${
            errors.name ? "border-red-400" : "border-border"
          }`}
          placeholder="My awesome project"
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
          className="w-full px-4 py-2 text-sm transition border rounded-xl bg-surface border-border focus:outline-none focus:ring-2 focus:ring-accent"
        >
          <option value="GitHub">GitHub Link</option>
        </select>
        {errors.projectType && (
          <p className="text-xs text-red-500">{errors.projectType.message}</p>
        )}
      </div>

      {/* Project link */}
      <div className="space-y-1">
        <label className="block text-sm font-medium">GitHub Link</label>
        <input
          {...register("projectLink")}
          className={`w-full rounded-xl border px-4 py-2 text-sm bg-surface focus:outline-none focus:ring-2 focus:ring-accent transition ${
            errors.projectLink ? "border-red-400" : "border-border"
          }`}
          placeholder="https://github.com/your/repo"
        />
        {errors.projectLink && (
          <p className="text-xs text-red-500">{errors.projectLink.message}</p>
        )}
      </div>

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
