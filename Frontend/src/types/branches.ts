export type BranchDetailed = {
  id: string;
  name: string;
  packageCount: number;
  vulnerablityCount: number;
  commitDate: string;
  scanDate: string;
};

export type Branch = {
  name: string;
  id: string;
  processStep: ProcessStep;
  processType: string;
};

export enum ProcessStep {
  Created = "Created",
  SbomCreation = "SbomCreation",
  SbomIngest = "SbomIngest",
  Processed = "Processed",
}

export enum ProcessStatus {
  Pending = "Pending",
  Success = "Success",
  Failed = "Failed",
}
