import type { NameCount } from "./packages";

export interface VulnerabilitiesDto {
  vulnerabilities: VulnerabilitySmallDto[];
  risks: NameCount[];
}

export interface VulnerabilitySmallDto {
  vulnerabilityId: string;
  severity: string;
  packageName: number;
}

export interface VulnerabilityDetailedDto {
  id: string;
  description: string;
  recommendation: string;
  severity: string;
}
