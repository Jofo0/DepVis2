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
};
