import type { Branch, BranchCommitsDto } from "@/types/branches";
import { createContext, useContext, useEffect, useState } from "react";
import { useGetProjectId } from "./useGetProjectId";
import { useGetProjectBranchesQuery } from "@/store/api/branchesApi";

type BranchContextType = {
  branch: Branch | null;
  commit: BranchCommitsDto | null;
  setBranch: React.Dispatch<React.SetStateAction<Branch | null>>;
  setCommit: React.Dispatch<React.SetStateAction<BranchCommitsDto | null>>;
  isLoading: boolean;
};

const BranchContext = createContext<BranchContextType>({
  branch: null,
  setBranch: () => {},
  commit: null,
  setCommit: () => {},
  isLoading: false,
});

export const BranchProvider = ({ children }: { children: React.ReactNode }) => {
  const id = useGetProjectId();
  const { isLoading } = useGetProjectBranchesQuery(id!);

  const [branch, setBranch] = useState<Branch | null>(null);
  const [commit, setCommit] = useState<BranchCommitsDto | null>(null);

  useEffect(() => {
    const stored = localStorage.getItem("branch");
    const storedCommit = localStorage.getItem("commit");

    if (stored) setBranch(JSON.parse(stored));
    if (storedCommit) setCommit(JSON.parse(storedCommit));
  }, []);

  useEffect(() => {
    if (branch) {
      // If branch changes, reset commit
      const stored = localStorage.getItem("branch");
      if (stored) {
        const retrievedBranch: Branch = JSON.parse(stored);
        if (branch.id !== retrievedBranch.id) {
          setCommit(null);
        }
      }

      localStorage.setItem("branch", JSON.stringify(branch));
    }
  }, [branch]);

  useEffect(() => {
    if (commit) {
      localStorage.setItem("commit", JSON.stringify(commit));
    }
  }, [commit]);

  return (
    <BranchContext.Provider
      value={{ branch, setBranch, isLoading, commit, setCommit }}
    >
      {children}
    </BranchContext.Provider>
  );
};

export const useBranch = () => useContext(BranchContext);
