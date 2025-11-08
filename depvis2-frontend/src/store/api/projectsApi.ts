import type {
  ProjectDto,
  CreateProjectDto,
  UpdateProjectDto,
  ProjectStatsDto,
  VulnerabilitySmallDto,
} from "../../types/projects";
import { projectsApi } from "../../store";
import type { GraphDataDto, PackageDetailedDto } from "../../types/packages";
import type { BranchDetailed, Branch } from "@/types/branches";

type IdWithOdata = {
  id: string;
  odata?: string;
};

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
    getProjectBranchesDetailed: builder.query<BranchDetailed[], IdWithOdata>({
      query: ({ id, odata }) =>
        `/${id}/branches/detailed${odata ? `?${odata}` : ""}`,
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
    }),
    getPackages: builder.query<PackageDetailedDto, IdWithOdata>({
      query: ({ id, odata }) => `/${id}/packages${odata ? `?${odata}` : ""}`,
    }),
    getProjectStats: builder.query<ProjectStatsDto, { id: string }>({
      query: ({ id }) => `/${id}/stats`,
      providesTags: (_res, _err, { id }) => [{ type: "Projects", id }],
    }),
    getVulnerabilities: builder.query<VulnerabilitySmallDto[], IdWithOdata>({
      query: ({ id, odata }) =>
        `/${id}/vulnerabilities${odata ? `?${odata}` : ""}`,
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
  useLazyGetPackagesQuery,
  useLazyGetVulnerabilitiesQuery,
} = projectApi;
