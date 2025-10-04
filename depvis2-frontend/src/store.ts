import { configureStore } from "@reduxjs/toolkit";
import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";

export const projectsApi = createApi({
  reducerPath: "projectsApi",
  baseQuery: fetchBaseQuery({
    baseUrl: "https://localhost:7185" + "/api/projects",
  }),
  endpoints: () => ({}),
  tagTypes: ["Projects"],
});

export const gitApi = createApi({
  reducerPath: "gitApi",
  baseQuery: fetchBaseQuery({
    baseUrl: "https://localhost:7185" + "/api/git",
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
