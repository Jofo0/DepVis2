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
  isTag: boolean;
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
  Created = "Created",
  SbomCreation = "SbomCreation",
  SbomIngest = "SbomIngest",
  Processed = "Processed",
  NotStarted = "NotStarted",
}

export enum ProcessStatus {
  Pending = "Pending",
  Success = "Success",
  Failed = "Failed",
}
