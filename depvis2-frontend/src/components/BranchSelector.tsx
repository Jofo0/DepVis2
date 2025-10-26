import { useGetProjectBranchesQuery } from "@/store/api/projectsApi";
import { useBranch } from "@/utils/hooks/BranchProvider";
import { useGetProjectId } from "@/utils/hooks/useGetProjectId";

import { useEffect } from "react";
import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectLabel,
  SelectTrigger,
  SelectValue,
} from "./ui/select";

const BranchSelector = () => {
  const id = useGetProjectId();
  const { data: branches, isLoading: branchesLoading } =
    useGetProjectBranchesQuery(id!);

  const { branch, setBranch } = useBranch();

  useEffect(() => {
    if (branch && branches && !branches?.find((x) => x.id === branch.id)) {
      setBranch(branches[0]);
    }
  }, [branches, branch]);

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
      <SelectContent>
        <SelectGroup>
          <SelectLabel>Branches</SelectLabel>
          {branches?.map((x) => (
            <SelectItem value={x.id} key={x.id}>
              {x.name}
            </SelectItem>
          ))}
        </SelectGroup>
      </SelectContent>
    </Select>
  );
};

export default BranchSelector;
