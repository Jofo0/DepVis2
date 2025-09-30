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
