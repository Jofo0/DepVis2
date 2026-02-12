export type ProjectDto = {
  id: string;
  name: string;
  projectType: string;
  processStep: string;
  processStatus: string;
  projectLink: string;
};

export type CreateProjectDto = {
  name: string;
  projectType: string;
  branches: string[];
  tags: string[];
  projectLink: string;
};

export type ProjectInfoDto = {
  name: string;
  projectType: string;
  branches: string[];
  tags: string[];
  projectLink: string;
  id: string;
};

export type UpdateProjectDto = {
  name: string;
  projectType: string;
  branches: string[];
  tags: string[];
  projectLink: string;
  id: string;
};

export type ProjectStatsDto = {
  packageCount: number;
  vulnerabilityCount: number;
};
