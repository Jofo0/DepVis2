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
  processType: ProcessStatus;
  isTag: boolean;
};

export type GetBranchesDto = {
  items: Branch[];
  totalCount: number;
  complete: number;
  sbomIngested: number;
  sbomGenerated: number;
  initiated: number;
};
export type BranchHistoryDto = {
  histories: BranchHistoryItem[];
  processingStep: ProcessStep;
};

export type BranchHistoryItem = {
  id: string;
  commitMessage: string;
  packageCount: number;
  vulnerablityCount: number;
  commitDate: string;
};

export enum ProcessStep {
  NotStarted = "NotStarted",
  Created = "Created",
  SbomCreation = "SbomCreation",
  SbomIngest = "SbomIngest",
  Processed = "Processed",
}

export const ProcessStepOrder: Record<ProcessStep, number> = {
  [ProcessStep.NotStarted]: 0,
  [ProcessStep.Created]: 1,
  [ProcessStep.SbomCreation]: 2,
  [ProcessStep.SbomIngest]: 3,
  [ProcessStep.Processed]: 4,
};

export enum ProcessStatus {
  Pending = "Pending",
  Success = "Success",
  Failed = "Failed",
}
