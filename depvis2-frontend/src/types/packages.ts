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
  version: string;
  ecosystem: string;
  vulnerable: boolean;
};

export type PackageRelationDto = {
  from: string;
  to: string;
};
