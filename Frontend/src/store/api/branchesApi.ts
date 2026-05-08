import { projectsApi } from "../../store";
import type {
  BranchDetailed,
  Branch,
  BranchHistoryDto,
  GetBranchesDto,
  BranchComparison,
} from "@/types/branches";

type IdWithOdata = {
  id: string;
  odata?: string;
};

export const projectApi = projectsApi.injectEndpoints({
  endpoints: (builder) => ({
    getProjectBranches: builder.query<GetBranchesDto, string>({
      query: (id) => `/${id}/branches`,
      providesTags: ["Branches"],
    }),
    reprocessBranch: builder.mutation<void, { projectId: string; id: string }>({
      query: (dto) => ({
        url: `/${dto.projectId}/branches/${dto.id}/process`,
        method: "POST",
      }),
      invalidatesTags: ["Projects", "Branches", "BranchHistory"],
    }),
    ingestBranchHistory: builder.mutation<
      void,
      { branchId: string; historyId: string, projectId: string }
    >({
      query: (dto) => ({
        url: `/${dto.projectId}/branches/${dto.branchId}/history/${dto.historyId}/ingest`,
        method: "POST",
      }),
      invalidatesTags: ["Projects", "Branches", "BranchHistory"],
    }),
    processBranchHistory: builder.mutation<Branch[], { branchId: string, projectId: string }>({
      query: ({ branchId, projectId }) => ({ url: `/${projectId}/branches/${branchId}/history`, method: "POST" }),
      invalidatesTags: ["BranchHistory"],
    }),
    getBranchHistory: builder.query<BranchHistoryDto, { branchId: string, projectId: string }>({
      query: ({ branchId, projectId }) => ({ url: `/${projectId}/branches/${branchId}/history`, method: "GET" }),
      providesTags: ["BranchHistory"],
    }),
    exportBranchHistory: builder.query<Blob, { branchId: string, projectId: string }>({
      query: ({ branchId, projectId }) => ({
        url: `/${projectId}/branches/${branchId}/history?$export=true`,
        method: "GET",
        responseHandler: (response) => response.blob(),
      }),
      providesTags: ["BranchHistory"],
    }),
    downloadBranchSbom: builder.query<Blob, { branchId: string, projectId: string }>({
      query: ({ branchId, projectId }) => ({
        url: `/${projectId}/branches/${branchId}/sbom/download`,
        method: "GET",
        responseHandler: (response) => response.blob(),
      }),
    }),
    getProjectBranchesDetailed: builder.query<BranchDetailed[], IdWithOdata>({
      query: ({ id, odata }) =>
        `/${id}/branches/detailed${odata ? `?${odata}` : ""}`,
      providesTags: ["Branches"],
    }),
    getProjectBranchesDetailedExport: builder.query<Blob, IdWithOdata>({
      query: ({ id, odata }) => ({
        responseHandler: (response) => response.blob(),
        url: `/${id}/branches/detailed?$export=true${odata ? `&${odata}` : ""}`,
      }),
    }),
    getBranchComparison: builder.query<
      BranchComparison,
      { branchId: string; compareToBranchId: string }
    >({
      query: ({ branchId, compareToBranchId }) => ({
        url: `/${branchId}/compare/${compareToBranchId}`,
      }),
    }),
  }),
});

export const {
  useLazyGetBranchComparisonQuery,
  useReprocessBranchMutation,
  useIngestBranchHistoryMutation,
  useGetBranchHistoryQuery,
  useProcessBranchHistoryMutation,
  useLazyExportBranchHistoryQuery,
  useGetProjectBranchesDetailedQuery,
  useGetProjectBranchesQuery,
  useLazyGetProjectBranchesDetailedExportQuery,
  useLazyDownloadBranchSbomQuery
} = projectApi;
