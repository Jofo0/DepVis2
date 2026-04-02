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
import {
  useGetProjectBranchesQuery,
  useIngestBranchHistoryMutation,
} from "@/store/api/branchesApi";
import {
  HistoryProcessStep,
  ProcessStatus,
  ProcessStep,
  type Branch,
  type BranchCommitsDto,
} from "@/types/branches";
import { Button } from "./ui/button";
import IconTooltip from "./IconTooltip";
import { BadgeInfo, Info, Loader } from "lucide-react";

type BranchSelectorProps = {
  onlyBranches?: boolean;
  hideCommits?: boolean;
  branch: Branch | null;
  setBranch: (branch: Branch) => void;
  commit: BranchCommitsDto | null;
  setCommit: (commit: BranchCommitsDto | null) => void;
};

export const GlobalBranchSelector = ({
  onlyBranches = false,
  hideCommits = false,
}: Omit<
  BranchSelectorProps,
  "branch" | "setBranch" | "commit" | "setCommit"
>) => {
  const { branch, setBranch, commit, setCommit } = useBranch();
  return (
    <BranchSelector
      onlyBranches={onlyBranches}
      hideCommits={hideCommits}
      branch={branch}
      setBranch={setBranch}
      commit={commit}
      setCommit={setCommit}
    />
  );
};

const BranchSelector = ({
  onlyBranches = false,
  hideCommits = false,
  branch,
  setBranch,
  commit,
  setCommit,
}: BranchSelectorProps) => {
  const id = useGetProjectId();
  const { data, isLoading: branchesLoading } = useGetProjectBranchesQuery(id!);

  const branches = useMemo(() => data?.items ?? [], [data?.items]);
  const filteredBranches = useMemo(
    () => branches.filter((x) => (onlyBranches ? !x.isTag : true)),
    [branches, onlyBranches],
  );

  useEffect(() => {
    if (!filteredBranches.length) {
      return;
    }
    if (
      (branch && !filteredBranches.find((x) => x.id === branch.id)) ||
      !branch
    ) {
      setBranch(filteredBranches[0]);
    }
  }, [filteredBranches, branch, setBranch]);

  const branchesOnly = useMemo(
    () =>
      filteredBranches
        ?.filter((b) => !b.isTag)
        .map((x) => (
          <SelectItem value={x.id} key={x.id}>
            {`${x.name} ${x.processStep !== ProcessStep.Processed ? "(Not Processed Yet)" : ""}`}
          </SelectItem>
        )) ?? [],
    [filteredBranches],
  );

  const tagsOnly = useMemo(
    () =>
      filteredBranches
        ?.filter((b) => b.isTag)
        .map((x) => (
          <SelectItem value={x.id} key={x.id}>
            {`${x.name} ${x.processStep !== ProcessStep.Processed ? "(Not Processed Yet)" : ""}`}
          </SelectItem>
        )) ?? [],
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
          value={filteredBranches.length === 0 ? "" : branch?.id || ""}
          onValueChange={(value) => {
            const selected = branches?.find((b) => b.id === value);
            if (selected) setBranch(selected);
          }}
          disabled={branchesLoading || filteredBranches.length === 0}
        >
          <SelectTrigger className="w-[180px]">
            <SelectValue
              placeholder={
                filteredBranches.length === 0
                  ? "No branches available"
                  : "Select a branch"
              }
            />
          </SelectTrigger>
          <SelectContent avoidCollisions={false} className="max-h-64">
            {branchesOnly.length > 0 && (
              <SelectGroup>
                <SelectLabel>Branches</SelectLabel>
                {branchesOnly}
              </SelectGroup>
            )}
            {tagsOnly.length > 0 && (
              <SelectGroup>
                <SelectLabel>Tags</SelectLabel>
                {tagsOnly}
              </SelectGroup>
            )}
          </SelectContent>
        </Select>
      </div>
      {!hideCommits && (
        <div className="flex flex-row  items-end gap-2">
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
          <HistoryIngestButton commit={commit} branchId={branch?.id} />
        </div>
      )}
    </div>
  );
};

const HistoryIngestButton = ({
  commit,
  branchId,
}: {
  branchId?: string;
  commit: BranchCommitsDto | null;
}) => {
  const id = useGetProjectId();
  const { refetch, isFetching: isFetchingBranches } =
    useGetProjectBranchesQuery(id!);
  const [mutate, { isLoading }] = useIngestBranchHistoryMutation();

  const handleIngest = async () => {
    if (!commit || !branchId) {
      return;
    }
    await mutate({ historyId: commit.commitId, branchId });
  };

  if (!commit || !branchId) {
    return null;
  }

  if (
    commit.processState === HistoryProcessStep.Processing &&
    commit.processStatus === ProcessStatus.Success
  ) {
    return (
      <div className="flex flex-row gap-2 items-center">
        <Button variant={"outline"} onClick={handleIngest} disabled={isLoading}>
          Ingest History Data
        </Button>
        <IconTooltip
          content="To save space, history data is not ingested automatically. Click the button to start the ingest process."
          trigger={<Info />}
        />
      </div>
    );
  }

  if (commit.processStatus === ProcessStatus.Success) {
    return (
      <IconTooltip
        content="History data ingested successfully."
        trigger={<Info />}
      />
    );
  } else if (commit.processStatus === ProcessStatus.Pending) {
    return (
      <div className="flex flex-row items-center" onClick={() => refetch()}>
        <IconTooltip
          content="History data is being ingested... Click the button to refresh the status."
          trigger={
            isFetchingBranches ? (
              <Loader className="animate-spin" />
            ) : (
              <BadgeInfo />
            )
          }
        />
      </div>
    );
  }
};

export default BranchSelector;
