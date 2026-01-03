import type {
  ProjectDto,
  CreateProjectDto,
  UpdateProjectDto,
  ProjectStatsDto,
} from "../../types/projects";
import { projectsApi } from "../../store";
import type {
  GraphDataDto,
  PackageDetailedDto,
  PackagesDetailedDto,
  Severity,
} from "../../types/packages";
import type { BranchDetailed, Branch } from "@/types/branches";
import type {
  VulnerabilitiesDto,
  VulnerabilityDetailedDto,
} from "@/types/vulnerabilities";

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
    getProjectBranchesDetailedExport: builder.query<Blob, IdWithOdata>({
      query: ({ id, odata }) => ({
        responseHandler: (response) => response.blob(),
        url: `/${id}/branches/detailed?$export=true${odata ? `&${odata}` : ""}`,
      }),
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
    getProjectGraph: builder.query<
      GraphDataDto,
      {
        id: string;
        packageId?: string;
        severityFilter?: Severity;
        showParents: boolean;
      }
    >({
      query: ({ id, packageId, severityFilter, showParents }) =>
        `/${id}/packages/graph/${packageId ? packageId : ""}?${
          severityFilter
            ? `severity=${severityFilter}&showAllParents=${showParents}`
            : ""
        }`,
    }),
    getPackages: builder.query<PackagesDetailedDto, IdWithOdata>({
      query: ({ id, odata }) => `/${id}/packages${odata ? `?${odata}` : ""}`,
    }),
    getPackagesExport: builder.query<Blob, IdWithOdata>({
      query: ({ id, odata }) => ({
        responseHandler: (response) => response.blob(),
        url: `/${id}/packages?$export=true${odata ? `&${odata}` : ""}`,
      }),
    }),
    getPackage: builder.query<PackageDetailedDto, string>({
      query: (id) => `/packages/${id}`,
    }),
    getProjectStats: builder.query<ProjectStatsDto, { id: string }>({
      query: ({ id }) => `/${id}/stats`,
      providesTags: (_res, _err, { id }) => [{ type: "Projects", id }],
    }),
    getVulnerabilities: builder.query<VulnerabilitiesDto, IdWithOdata>({
      query: ({ id, odata }) =>
        `/${id}/vulnerabilities${odata ? `?${odata}` : ""}`,
    }),
    getVulnerabilitiesExport: builder.query<Blob, IdWithOdata>({
      query: ({ id, odata }) => ({
        responseHandler: (response) => response.blob(),
        url: `/${id}/vulnerabilities?$export=true${odata ? `&${odata}` : ""}`,
      }),
    }),
    getVulnerability: builder.query<VulnerabilityDetailedDto, string>({
      query: (id) => `/vulnerabilities/${id}`,
    }),
  }),
});

export const {
  useGetProjectsQuery,
  useLazyGetPackageQuery,
  useGetProjectQuery,
  useCreateProjectMutation,
  useUpdateProjectMutation,
  useGetProjectBranchesDetailedQuery,
  useDeleteProjectMutation,
  useGetProjectBranchesQuery,
  useGetProjectStatsQuery,
  useGetProjectGraphQuery,
  useLazyGetVulnerabilityQuery,
  useLazyGetVulnerabilitiesExportQuery,
  useLazyGetPackagesQuery,
  useLazyGetPackagesExportQuery,
  useLazyGetVulnerabilitiesQuery,
  useLazyGetProjectBranchesDetailedExportQuery,
} = projectApi;
