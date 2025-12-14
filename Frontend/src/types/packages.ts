import type { VulnerabilityDetailedDto } from "./vulnerabilities";

export type GraphDataDto = {
  packages: PackageDto[];
  relationships: PackageRelationDto[];
};

export type Severity = "low" | "medium" | "high" | "critical" | "None";

export type PackageDto = {
  name: string;
  severity?: Severity;
  id: string;
};

export type PackagesDetailedDto = {
  packageItems: PackageItemDto[];
  vulnerabilities: NameCount[];
  ecoSystems: NameCount[];
};

export type PackageDetailedDto = {
  name: string;
  id: string;
  version: string;
  ecosystem: string;
  vulnerabilities: VulnerabilityDetailedDto[];
};

export type NameCount = {
  name: string;
  count: number;
};
export type PackageItemDto = {
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
