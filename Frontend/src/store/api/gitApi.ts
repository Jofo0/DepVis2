import { gitApi } from "../../store";
import type { GitInformationDto } from "../../types/git";

export const gitCalls = gitApi.injectEndpoints({
  endpoints: (builder) => ({
    getGitInformation: builder.query<GitInformationDto, string>({
      query: (gitHubUrl) => `/${gitHubUrl}`,
      providesTags: ["GitInformation"],
    }),
  }),
});

export const { useLazyGetGitInformationQuery } = gitCalls;
