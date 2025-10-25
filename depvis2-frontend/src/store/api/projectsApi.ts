import type {
  ProjectDto,
  CreateProjectDto,
  UpdateProjectDto,
  ProjectStatsDto,
} from "../../types/projects";
import { projectsApi } from "../../store";
import type { GraphDataDto } from "../../types/packages";
import type { BranchDetailed, Branch } from "@/types/branches";

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
    getProjectBranches: builder.query<Branch[], string>({
      query: (id) => `/${id}/branches`,
    }),
    getProjectBranchesDetailed: builder.query<BranchDetailed[], string>({
      query: (id) => `/${id}/branches/detailed`,
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
    getProjectGraph: builder.query<GraphDataDto, { id: string }>({
      query: ({ id }) => `/${id}/packages/graph`,
      providesTags: (_res, _err, { id }) => [{ type: "Projects", id }],
    }),
    getProjectStats: builder.query<ProjectStatsDto, { id: string }>({
      query: ({ id }) => `/${id}/stats`,
      providesTags: (_res, _err, { id }) => [{ type: "Projects", id }],
    }),
  }),
});

export const {
  useGetProjectsQuery,
  useGetProjectQuery,
  useCreateProjectMutation,
  useUpdateProjectMutation,
  useGetProjectBranchesDetailedQuery,
  useDeleteProjectMutation,
  useGetProjectBranchesQuery,
  useGetProjectStatsQuery,
  useGetProjectGraphQuery,
} = projectApi;
