import { projectsApi } from "../../store";
import type {
  BranchDetailed,
  Branch,
  BranchHistoryDto,
} from "@/types/branches";

type IdWithOdata = {
  id: string;
  odata?: string;
};

export const projectApi = projectsApi.injectEndpoints({
  endpoints: (builder) => ({
    getProjectBranches: builder.query<Branch[], string>({
      query: (id) => `/${id}/branches`,
      providesTags: ["Branches"],
    }),
    processBranchHistory: builder.mutation<Branch[], string>({
      query: (id) => ({ url: `/${id}/branches/history`, method: "POST" }),
      invalidatesTags: ["BranchHistory"],
    }),
    getBranchHistory: builder.query<BranchHistoryDto, string>({
      query: (id) => ({ url: `/${id}/branches/history`, method: "GET" }),
      providesTags: ["BranchHistory"],
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
  }),
});

export const {
  useGetBranchHistoryQuery,
  useProcessBranchHistoryMutation,
  useGetProjectBranchesDetailedQuery,
  useGetProjectBranchesQuery,
  useLazyGetProjectBranchesDetailedExportQuery,
} = projectApi;
