import type { NameCount } from "./packages";

export type VulnerabilitiesDto = {
  vulnerabilities: VulnerabilitySmallDto[];
  risks: NameCount[];
  depths: NameCount[];
};

export type VulnerabilitySmallDto = {
  vulnerabilityId: string;
  severity: string;
  packageName: number;
  depth: string;
  packageId: string;
};

export type VulnerabilityDetailedDto = {
  id: string;
  description: string;
  recommendation: string;
  severity: string;
  cwes: number[];
  references: string[];
};
