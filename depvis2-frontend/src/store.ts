import { configureStore } from "@reduxjs/toolkit";
import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { API_BASE_URL } from "./config";

export const projectsApi = createApi({
  reducerPath: "projectsApi",
  baseQuery: fetchBaseQuery({
    baseUrl: API_BASE_URL + "/api/projects",
  }),
  endpoints: () => ({}),
  tagTypes: ["Projects"],
});

export const gitApi = createApi({
  reducerPath: "gitApi",
  baseQuery: fetchBaseQuery({
    baseUrl: API_BASE_URL + "/api/git",
  }),
  endpoints: () => ({}),
  tagTypes: ["GitInformation"],
});

export const store = configureStore({
  reducer: {
    [projectsApi.reducerPath]: projectsApi.reducer,
    [gitApi.reducerPath]: gitApi.reducer,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware()
      .concat(projectsApi.middleware)
      .concat(gitApi.middleware),
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
