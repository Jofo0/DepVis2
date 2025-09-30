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
  projectLink: string;
};

export type UpdateProjectDto = {
  name: string;
  projectType: string;
  processStatus: string;
  projectLink: string;
};

// types/graph.ts
export interface GraphDataDto {
  packages: PackageDto[];
  relationships: PackageRelationDto[];
}

export interface PackageDto {
  name: string;
  id: string;
}

export interface PackageRelationDto {
  from: string;
  to: string;
}
