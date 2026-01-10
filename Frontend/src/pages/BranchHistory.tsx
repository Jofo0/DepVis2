import BranchSelector from "@/components/BranchSelector";
import { Button } from "@/components/ui/button";
import { useProcessBranchHistoryMutation } from "@/store/api/projectsApi";
import { useBranch } from "@/utils/hooks/BranchProvider";

const BranchHistory = () => {
  const { branch } = useBranch();
  const [mutate] = useProcessBranchHistoryMutation();

  const onProcessHistoryClick = async () => {
    if (branch) {
      await mutate(branch.id);
    }
  };

  return (
    <div className="flex flex-col gap-3 w-full h-full">
      <div className="flex flex-row gap-10 w-full h-full justify-evenly">
        <BranchSelector />
        <Button onClick={onProcessHistoryClick} />
      </div>
    </div>
  );
};

export default BranchHistory;
