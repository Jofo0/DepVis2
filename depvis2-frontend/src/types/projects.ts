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

export type UpdateProjectDto = {
  name: string;
  projectType: string;
  processStatus: string;
  projectLink: string;
};

export interface ProjectStatsDto {
  packageCount: number;
  vulnerabilityCount: number;
}

export type ProjectBranchDto = {
  name: string;
  id: string;
};
