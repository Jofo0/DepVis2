import type {
  ProjectDto,
  CreateProjectDto,
  UpdateProjectDto,
} from "../types/projects";
import { projectsApi } from "../store";

export const projectApi = projectsApi.injectEndpoints({
  endpoints: (builder) => ({
    getProjects: builder.query<ProjectDto[], void>({
      query: () => "/",
      providesTags: ["Projects"],
    }),
    getProject: builder.query<ProjectDto, string>({
      query: (id) => `/${id}`,
      providesTags: (_res, _err, id) => [{ type: "Projects", id }],
    }),
    createProject: builder.mutation<ProjectDto, CreateProjectDto>({
      query: (dto) => ({
        url: "/",
        method: "POST",
        body: dto,
      }),
      invalidatesTags: ["Projects"],
    }),
    updateProject: builder.mutation<
      void,
      { id: string; dto: UpdateProjectDto }
    >({
      query: ({ id, dto }) => ({
        url: `/${id}`,
        method: "PUT",
        body: dto,
      }),
      invalidatesTags: (_res, _err, { id }) => [
        "Projects",
        { type: "Projects", id },
      ],
    }),
    deleteProject: builder.mutation<void, string>({
      query: (id) => ({
        url: `/${id}`,
        method: "DELETE",
      }),
      invalidatesTags: ["Projects"],
    }),
    publishTest: builder.query<void, void>({
      query: () => "/test",
    }),
  }),
});

export const {
  useGetProjectsQuery,
  useGetProjectQuery,
  useCreateProjectMutation,
  useUpdateProjectMutation,
  useDeleteProjectMutation,
  usePublishTestQuery,
} = projectApi;
