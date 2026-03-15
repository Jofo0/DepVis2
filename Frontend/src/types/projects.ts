export type ProjectDto = {
  id: string;
  name: string;
  projectLink: string;
  ecoSystems: string[];
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
