import { useGetProjectBranchesQuery } from "@/store/api/projectsApi";
import type { Branch } from "@/types/branches";
import { createContext, useContext, useEffect, useState } from "react";
import { useGetProjectId } from "./useGetProjectId";

type BranchContextType = {
  branch: Branch | null;
  setBranch: React.Dispatch<React.SetStateAction<Branch | null>>;
  isLoading: boolean;
};

const BranchContext = createContext<BranchContextType>({
  branch: null,
  setBranch: () => {},
  isLoading: false,
});

export const BranchProvider = ({ children }: { children: React.ReactNode }) => {
  const id = useGetProjectId();
  const { isLoading } = useGetProjectBranchesQuery(id!);

  const [branch, setBranch] = useState<Branch | null>(null);
  useEffect(() => {
    const stored = localStorage.getItem("branch");
    if (stored) setBranch(JSON.parse(stored));
  }, []);

  useEffect(() => {
    if (branch) {
      localStorage.setItem("branch", JSON.stringify(branch));
    }
  }, [branch]);

  return (
    <BranchContext.Provider value={{ branch, setBranch, isLoading }}>
      {children}
    </BranchContext.Provider>
  );
};

export const useBranch = () => useContext(BranchContext);
