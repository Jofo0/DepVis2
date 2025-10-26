import type { Branch } from "@/types/branches";
import { createContext, useContext, useEffect, useState } from "react";

type BranchContextType = {
  branch: Branch | null;
  setBranch: React.Dispatch<React.SetStateAction<Branch | null>>;
};

const BranchContext = createContext<BranchContextType>({
  branch: null,
  setBranch: () => {},
});

export const BranchProvider = ({ children }: { children: React.ReactNode }) => {
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
    <BranchContext.Provider value={{ branch, setBranch }}>
      {children}
    </BranchContext.Provider>
  );
};

export const useBranch = () => useContext(BranchContext);
