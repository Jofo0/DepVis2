import type {
  ProjectDto,
  CreateProjectDto,
  ProjectStatsDto,
  ProjectInfoDto,
} from "../../types/projects";
import { projectsApi } from "../../store";
import type {
  GraphDataDto,
  PackageDetailedDto,
  PackagesDetailedDto,
  Severity,
} from "../../types/packages";
import type {
  VulnerabilitiesDto,
  VulnerabilityDetailedDto,
} from "@/types/vulnerabilities";

type IdWithOdata = {
  id: string;
  odata?: string;
  commitId?: string;
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
    getProjectInfo: builder.query<ProjectInfoDto, string>({
      query: (id) => `/${id}/info`,
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
    editProject: builder.mutation<
      ProjectDto,
      CreateProjectDto & { id: string }
    >({
      query: (dto) => ({
        url: "/" + dto.id,
        method: "PUT",
        body: dto,
      }),
      invalidatesTags: ["Projects", "Branches"],
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
        commitId?: string;
        packageId?: string;
        severityFilter?: Severity;
        showParents: boolean;
      }
    >({
      query: ({ id, packageId, commitId, severityFilter, showParents }) =>
        `/${id}/packages/graph/${packageId ? packageId : ""}?${
          severityFilter
            ? `severity=${severityFilter}&showAllParents=${showParents}`
            : ""
        }${commitId ? `&commitId=${commitId}` : ""}`,
    }),
    getProjectGraphExport: builder.query<
      Blob,
      {
        id: string;
        packageId?: string;
        severityFilter?: Severity;
        commitId?: string;
        showParents: boolean;
      }
    >({
      query: ({ id, packageId, severityFilter, commitId, showParents }) => ({
        responseHandler: (response) => response.blob(),
        url: `/${id}/packages/graph/${packageId ? packageId : ""}?$export=true${
          severityFilter
            ? `&severity=${severityFilter}&showAllParents=${showParents}`
            : ""
        }${commitId ? `&commitId=${commitId}` : ""}`,
      }),
    }),
    getPackages: builder.query<PackagesDetailedDto, IdWithOdata>({
      query: ({ id, odata, commitId }) =>
        `/${id}/packages?${odata ? `${odata}` : ""}${commitId ? `&commitId=${commitId}` : ""}`,
    }),
    getPackagesExport: builder.query<Blob, IdWithOdata>({
      query: ({ id, odata, commitId }) => ({
        responseHandler: (response) => response.blob(),
        url: `/${id}/packages?$export=true${odata ? `&${odata}` : ""}${commitId ? `&commitId=${commitId}` : ""}`,
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
      query: ({ id, odata, commitId }) =>
        `/${id}/vulnerabilities?${odata ? `${odata}` : ""}${commitId ? `&commitId=${commitId}` : ""}`,
    }),
    getVulnerabilitiesExport: builder.query<Blob, IdWithOdata>({
      query: ({ id, odata, commitId }) => ({
        responseHandler: (response) => response.blob(),
        url: `/${id}/vulnerabilities?$export=true${odata ? `&${odata}` : ""}${commitId ? `&commitId=${commitId}` : ""}`,
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
  useGetProjectInfoQuery,
  useGetProjectQuery,
  useCreateProjectMutation,
  useEditProjectMutation,
  useDeleteProjectMutation,
  useGetProjectStatsQuery,
  useGetProjectGraphQuery,
  useLazyGetVulnerabilityQuery,
  useLazyGetVulnerabilitiesExportQuery,
  useLazyGetPackagesQuery,
  useLazyGetProjectGraphExportQuery,
  useLazyGetPackagesExportQuery,
  useLazyGetVulnerabilitiesQuery,
} = projectApi;
