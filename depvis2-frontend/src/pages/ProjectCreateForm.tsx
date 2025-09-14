import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { useCreateProjectMutation } from "../services/projectsApi";
import type { CreateProjectDto, ProjectDto } from "../types/projects";

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

export type ProjectCreateFormProps = {
  onSuccess?: (created: ProjectDto) => void;
};

const ProjectCreateForm = ({ onSuccess }: ProjectCreateFormProps) => {
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

  const onSubmit = async (values: FormValues) => {
    try {
      const payload: CreateProjectDto = {
        name: values.name,
        projectType: values.projectType,
        projectLink: values.projectLink || "",
      };

      const result = await createProject(payload).unwrap();

      // simple success handling
      reset();
      onSuccess?.(result);
    } catch (err) {
      // Basic error handling; you can plug in a toast library here
      console.error("Failed to create project", err);
      // If you want nicer UX, show a toast or set an error state
    }
  };

  return (
    <form
      onSubmit={handleSubmit(onSubmit)}
      className="w-full max-w-lg p-6 space-y-4 bg-white shadow-md dark:bg-slate-800 rounded-2xl"
    >
      <h3 className="text-lg font-semibold">Create project</h3>

      <div>
        <label className="block mb-1 text-sm font-medium">Name</label>
        <input
          {...register("name")}
          className={`w-full rounded-lg border px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-400 ${
            errors.name ? "border-red-400" : "border-slate-200"
          }`}
          placeholder="My awesome project"
        />
        {errors.name && (
          <p className="mt-1 text-xs text-red-500">{errors.name.message}</p>
        )}
      </div>

      <div>
        <label className="block mb-1 text-sm font-medium">Project type</label>
        <select
          {...register("projectType")}
          className="w-full px-3 py-2 text-sm border rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-400 border-slate-200"
        >
          <option value="GitHub">GitHub Link</option>
        </select>
        {errors.projectType && (
          <p className="mt-1 text-xs text-red-500">
            {errors.projectType.message}
          </p>
        )}
      </div>

      <div>
        <label className="block mb-1 text-sm font-medium">GitHub Link</label>
        <input
          {...register("projectLink")}
          className={`w-full rounded-lg border px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-400 ${
            errors.projectLink ? "border-red-400" : "border-slate-200"
          }`}
          placeholder="https://github.com/your/repo"
        />
        {errors.projectLink && (
          <p className="mt-1 text-xs text-red-500">
            {errors.projectLink.message}
          </p>
        )}
      </div>

      <div className="flex items-center justify-end pt-2 space-x-2">
        <button
          type="button"
          onClick={() => reset()}
          className="px-4 py-2 text-sm border rounded-lg"
        >
          Reset
        </button>

        <button
          type="submit"
          disabled={isLoading}
          className="px-4 py-2 text-sm text-white bg-indigo-600 rounded-lg hover:bg-indigo-700 disabled:opacity-60"
        >
          {isLoading ? "Creating..." : "Create Project"}
        </button>
      </div>
    </form>
  );
};

export default ProjectCreateForm;
