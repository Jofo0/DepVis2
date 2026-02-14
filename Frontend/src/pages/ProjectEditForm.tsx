import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  useEditProjectMutation,
  useGetProjectInfoQuery,
} from "../store/api/projectsApi";
import type { CreateProjectDto } from "../types/projects";
import { useNavigate } from "react-router-dom";
import { useLazyGetGitInformationQuery } from "../store/api/gitApi";
import { DevInput, InputButton } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import MultiSelection from "@/components/MultiSelection";
import { Card, CardDescription, CardHeader } from "@/components/ui/card";
import { useState, useEffect } from "react";
import { ChartLoader } from "@/components/chart/PieCustomChart";

const ProjectSchema = z
  .object({
    name: z.string().min(1, "Name is required"),
    projectType: z.enum(["GitHub"]),
    projectLink: z.url("Must be a valid URL"),
    selectedBranches: z.array(z.string()).optional(),
    selectedTags: z.array(z.string()).optional(),
  })
  .refine(
    (data) =>
      (data.selectedBranches?.length ?? 0) > 0 ||
      (data.selectedTags?.length ?? 0) > 0,
    {
      path: ["selectedBranches", "selectedTags"],
      message: "At least one branch or tag must be selected",
    },
  );

type FormValues = z.infer<typeof ProjectSchema>;

type ProjectEditFormProps = {
  projectId: string;
} & React.HTMLAttributes<HTMLDivElement>;

const ProjectEditForm = ({ projectId, ...props }: ProjectEditFormProps) => {
  const { data, isLoading: isLoadingProjectData } =
    useGetProjectInfoQuery(projectId);
  const [editProject, { isLoading }] = useEditProjectMutation();
  const [
    retrieveGitInfo,
    { isFetching: gitLoading, data: gitData, error: gitError },
  ] = useLazyGetGitInformationQuery();

  const {
    register,
    handleSubmit,
    formState: { errors },
    watch,
    setValue,
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
  const navigate = useNavigate();
  const [hasGitData, setHasGitData] = useState(false);

  useEffect(() => {
    if (data) {
      setValue("name", data.name);
      setValue("projectLink", data.projectLink);
      setValue("selectedBranches", data.branches ?? []);
      setValue("selectedTags", data.tags ?? []);

      handleFetchGitLink(data.projectLink);
    }
  }, [data, setValue]);

  const onSubmit = async (values: FormValues) => {
    try {
      const payload: CreateProjectDto & { id: string } = {
        name: values.name,
        projectType: values.projectType,
        projectLink: values.projectLink || "",
        branches: values.selectedBranches ?? [],
        tags: values.selectedTags ?? [],
        id: projectId,
      };

      await editProject(payload);
      navigate(`/${projectId}`);
    } catch (err) {
      console.error("Failed to update project", err);
    }
  };

  const selectedBranches = watch("selectedBranches");
  const selectedTags = watch("selectedTags");
  const projectLink = watch("projectLink");

  const handleFetchGitLink = async (link: string) => {
    await retrieveGitInfo(encodeURIComponent(link));
    setHasGitData(true);
  };

  const handleFetchGit = async () => {
    if (!projectLink) return;
    await retrieveGitInfo(encodeURIComponent(projectLink));
    setHasGitData(true);
  };

  const removedBranches =
    data?.branches?.filter((b) => !(selectedBranches ?? []).includes(b)) ?? [];
  const removedTags =
    data?.tags?.filter((t) => !(selectedTags ?? []).includes(t)) ?? [];

  return (
    <Card {...props}>
      <form
        onSubmit={handleSubmit(onSubmit)}
        className="flex flex-col gap-5 p-3"
      >
        <CardHeader className="flex flex-row justify-between w-full">
          <h1>Edit Project</h1>
        </CardHeader>
        <CardDescription className="pl-6">
          Removing branches or tags will remove them from the project and all
          associated data will be lost.
        </CardDescription>
        {isLoadingProjectData ? (
          <ChartLoader />
        ) : (
          <>
            <DevInput
              disabled
              title="Name"
              error={errors.name?.message}
              {...register("name")}
            />

            <div className="space-y-1">
              <div className="flex gap-2 w-full items-end">
                <InputButton
                  disabled
                  title="GitHub Link"
                  {...register("projectLink")}
                  className="w-full"
                  placeholder="https://github.com/owner/repo"
                  error={errors.projectLink?.message}
                  buttonText="Fetch Git Info"
                  isLoading={gitLoading}
                  disabledButton={
                    !projectLink || !!errors.projectLink || gitLoading
                  }
                  onClick={handleFetchGit}
                />
              </div>

              {gitError && (
                <p className="text-xs text-red-500">
                  Couldn’t fetch repository info. Check the URL or your
                  credentials.
                </p>
              )}
            </div>

            {gitData && hasGitData && (
              <div className="flex justify-between gap-4 max-w-full">
                <MultiSelection
                  onValuesChange={(values) =>
                    setValue("selectedBranches", values)
                  }
                  values={selectedBranches ?? []}
                  title="Branches"
                  placeholder="Select branches..."
                  data={gitData?.branches ?? []}
                  className="w-full max-w-1/2"
                />
                <MultiSelection
                  onValuesChange={(values) => setValue("selectedTags", values)}
                  values={selectedTags ?? []}
                  title="Tags"
                  placeholder="Select tags..."
                  data={gitData?.tags ?? []}
                  className="w-full max-w-1/2"
                />
              </div>
            )}

            <div className="flex items-center justify-end pt-4 space-x-3">
              <Button
                type="submit"
                disabled={
                  isLoading ||
                  (selectedBranches?.length === 0 && selectedTags?.length === 0)
                }
              >
                {isLoading ? "Updating..." : "Update Project"}
              </Button>
            </div>
          </>
        )}
      </form>
      {(removedBranches.length > 0 || removedTags.length > 0) && (
        <div className="pt-2 border-t">
          <h3 className="text-sm font-medium">Will be removed</h3>
          <p className="text-xs text-muted-foreground">
            These items will be removed when you click “Update Project”.
          </p>

          <div className="mt-3 grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <div className="text-xs font-medium mb-2">Branches</div>
              {removedBranches.length === 0 ? (
                <div className="text-xs text-muted-foreground">None</div>
              ) : (
                <div className="flex flex-wrap gap-2">
                  {removedBranches.map((b) => (
                    <span
                      key={b}
                      className="text-xs px-2 py-1 rounded-full border bg-muted"
                    >
                      {b}
                    </span>
                  ))}
                </div>
              )}
            </div>

            <div>
              <div className="text-xs font-medium mb-2">Tags</div>
              {removedTags.length === 0 ? (
                <div className="text-xs text-muted-foreground">None</div>
              ) : (
                <div className="flex flex-wrap gap-2">
                  {removedTags.map((t) => (
                    <span
                      key={t}
                      className="text-xs px-2 py-1 rounded-full border bg-muted"
                    >
                      {t}
                    </span>
                  ))}
                </div>
              )}
            </div>
          </div>
        </div>
      )}
    </Card>
  );
};

export default ProjectEditForm;
