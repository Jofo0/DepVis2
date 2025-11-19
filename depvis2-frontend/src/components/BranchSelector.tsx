import { useGetProjectBranchesQuery } from "@/store/api/projectsApi";
import { useBranch } from "@/utils/hooks/BranchProvider";
import { useGetProjectId } from "@/utils/hooks/useGetProjectId";

import { useEffect, useMemo } from "react";
import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectLabel,
  SelectTrigger,
  SelectValue,
} from "./ui/select";
import { ProcessStep } from "@/types/branches";

const BranchSelector = () => {
  const id = useGetProjectId();
  const { data: branches, isLoading: branchesLoading } =
    useGetProjectBranchesQuery(id!);

  const { branch, setBranch } = useBranch();

  useEffect(() => {
    if (!branches?.length) return;
    if (branch && !branches.find((x) => x.id === branch.id)) {
      setBranch(branches[0]);
    }
  }, [branches, branch, setBranch]);

  const items = useMemo(
    () =>
      branches
        ?.filter((b) => b.processStep === ProcessStep.Processed)
        .map((x) => (
          <SelectItem value={x.id} key={x.id}>
            {x.name}
          </SelectItem>
        )) ?? null,
    [branches]
  );

  return (
    <Select
      value={branch?.id || ""}
      onValueChange={(value) => {
        const selected = branches?.find((b) => b.id === value);
        if (selected) setBranch(selected);
      }}
      disabled={branchesLoading}
    >
      <SelectTrigger className="w-[180px]">
        <SelectValue placeholder="Select a branch" />
      </SelectTrigger>
      <SelectContent avoidCollisions={false} className="max-h-64">
        <SelectGroup>
          <SelectLabel>
            {items?.length === 0
              ? "No Branches/Tags processed yet"
              : "Branches/Tags"}
          </SelectLabel>
          {items}
        </SelectGroup>
      </SelectContent>
    </Select>
  );
};

export default BranchSelector;
