export type GraphDataDto = {
  packages: PackageDto[];
  relationships: PackageRelationDto[];
};

export type PackageDto = {
  name: string;
  id: string;
};

export type PackageDetailedDto = {
  name: string;
  id: string;
  commitDate: string;
  commitMessage: string;
  commitSha: string;
  scanDate: string;
  ecosystem: string;
  vulnerable: boolean;
};

export type PackageRelationDto = {
  from: string;
  to: string;
};
