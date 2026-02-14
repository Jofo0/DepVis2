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
import { useGetProjectBranchesQuery } from "@/store/api/branchesApi";

type BranchSelectorProps = {
  onlyBranches?: boolean;
  hideCommits?: boolean;
};

const BranchSelector = ({
  onlyBranches = false,
  hideCommits = false,
}: BranchSelectorProps) => {
  const id = useGetProjectId();
  const { data, isLoading: branchesLoading } = useGetProjectBranchesQuery(id!);

  const { branch, setBranch, commit, setCommit } = useBranch();

  const branches = data?.items;
  const filteredBranches = branches?.filter((x) =>
    onlyBranches ? !x.isTag : true,
  );

  useEffect(() => {
    if (!filteredBranches?.length) {
      return;
    }
    if (
      (branch && !filteredBranches.find((x) => x.id === branch.id)) ||
      !branch
    ) {
      setBranch(filteredBranches[0]);
    } else {
      const updatedBranch = filteredBranches.find((x) => x.id === branch.id);
      if (updatedBranch) {
        setBranch(updatedBranch);
      }
    }
  }, [filteredBranches, branch, setBranch, onlyBranches]);

  const branchesOnly = useMemo(
    () =>
      filteredBranches
        ?.filter((b) => b.processStep === ProcessStep.Processed && !b.isTag)
        .map((x) => (
          <SelectItem value={x.id} key={x.id}>
            {x.name}
          </SelectItem>
        )) ?? null,
    [filteredBranches],
  );

  const tagsOnly = useMemo(
    () =>
      filteredBranches
        ?.filter((b) => b.processStep === ProcessStep.Processed && b.isTag)
        .map((x) => (
          <SelectItem value={x.id} key={x.id}>
            {x.name}
          </SelectItem>
        )) ?? null,
    [filteredBranches],
  );

  const commits = useMemo(
    () =>
      branch?.commits?.map((c) => (
        <SelectItem value={c.commitId} key={c.commitId}>
          {c.commitName}
        </SelectItem>
      )),
    [branch],
  );

  return (
    <div className="flex flex-row gap-2">
      <div className="flex flex-col">
        <div className="text-gray-700 text-sm">
          Select a branch{!onlyBranches && "/tag"}
        </div>
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
                {branchesOnly?.length !== 0
                  ? "Branches"
                  : onlyBranches && "No processed branches"}
              </SelectLabel>
              {branchesOnly}
            </SelectGroup>
            <SelectGroup>
              <SelectLabel>{tagsOnly?.length !== 0 && "Tags"}</SelectLabel>
              {tagsOnly}
            </SelectGroup>
          </SelectContent>
        </Select>
      </div>
      {!hideCommits && (
        <div className="flex flex-col">
          <div className="text-gray-700 text-sm">Select a Commit</div>
          <Select
            value={commit?.commitId || ""}
            onValueChange={(value) => {
              const selected = branch?.commits?.find(
                (c) => c.commitId === value,
              );
              if (selected) setCommit(selected);
            }}
            disabled={branch === null || branch?.commits?.length === 0}
          >
            <SelectTrigger className="w-[180px]">
              <SelectValue
                placeholder={
                  branch?.isTag
                    ? "No commits available"
                    : branch?.commits?.length === 0
                      ? "History not processed"
                      : "Select a commit"
                }
              />
            </SelectTrigger>
            <SelectContent avoidCollisions={false} className="max-h-64">
              <SelectGroup>
                <SelectLabel>Commits</SelectLabel>
                {commits}
              </SelectGroup>
            </SelectContent>
          </Select>
        </div>
      )}
    </div>
  );
};

export default BranchSelector;
