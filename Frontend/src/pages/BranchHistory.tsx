import BranchSelector from "@/components/BranchSelector";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import {
  useGetBranchHistoryQuery,
  useProcessBranchHistoryMutation,
} from "@/store/api/branchesApi";
import { ProcessStep } from "@/types/branches";
import { useBranch } from "@/utils/hooks/BranchProvider";
import { RefreshCcw } from "lucide-react";

const BranchHistory = () => {
  return (
    <div className="flex flex-col gap-3 w-full h-full pb-12">
      <div className="flex flex-col gap-10 w-full h-full justify-start ">
        <BranchSelector onlyBranches />
        <div className="flex flex-col items-center justify-center w-full h-full">
          <History />
        </div>
      </div>
    </div>
  );
};

const History = () => {
  const { branch } = useBranch();
  const {
    isFetching: isLoading,
    data,
    refetch,
  } = useGetBranchHistoryQuery(branch ? branch.id : "", { skip: !branch });
  const [mutate] = useProcessBranchHistoryMutation();

  const onProcessHistoryClick = async () => {
    if (branch) {
      await mutate(branch.id);
    }
  };

  if (!branch || isLoading || !data)
    return <div>Loading branch history...</div>;

  if (data?.processingStep === ProcessStep.NotStarted)
    return (
      <Card className="flex flex-col justify-center items-center w-1/3 self-center ">
        <div className="p-4">Branch History has not been processed yet</div>
        <Button onClick={onProcessHistoryClick} variant={"outline"}>
          Process Branch History
        </Button>
      </Card>
    );

  if (data?.processingStep !== ProcessStep.Processed)
    return (
      <Card className="flex flex-col justify-center items-center w-1/3 self-center ">
        <div className="p-4">Branch History is being processed</div>
        <Button variant={"ghost"} onClick={refetch}>
          <RefreshCcw />
        </Button>
      </Card>
    );

  return (
    <Card>
      {data.histories.map((item) => (
        <div key={item.id} className="border-b p-4 last:border-0">
          <div className="font-bold">{item.commitMessage}</div>
        </div>
      ))}
    </Card>
  );
};

export default BranchHistory;
