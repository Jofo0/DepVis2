import { Button } from "@/components/ui/button";
import { Card, CardDescription, CardHeader } from "@/components/ui/card";
import { useGetProjectBranchesQuery } from "@/store/api/branchesApi";
import { ProcessStep, ProcessStepOrder } from "@/types/branches";
import { ArrowBigRightDash, RefreshCcw } from "lucide-react";
import { useParams } from "react-router-dom";
import ProcessingTab from "./Processing/ProcessingTab";
import { useEffect, useState } from "react";
import BranchProcessingCard from "./Processing/BranchProcessingCard";
import InitialProcessStepUtil from "@/utils/InitialProcessStepUtil";

const Processing = () => {
  const { id } = useParams<{ id: string }>();
  const {
    data,
    isFetching: isLoading,
    refetch,
  } = useGetProjectBranchesQuery(id!);
  const [filter, setFilter] = useState<ProcessStep>(
    InitialProcessStepUtil(data)
  );
  const branches = data?.items ?? [];

  useEffect(() => {
    const initialStep = InitialProcessStepUtil(data);
    setFilter(initialStep);
  }, [data]);

  return (
    <Card className="pb-10">
      <CardHeader>
        <div className="flex flex-col">
          <div className="flex flex-row gap-2 items-center">
            Processing Statistics
            <Button
              variant={"ghost"}
              onClick={() => refetch()}
              disabled={isLoading}
            >
              <RefreshCcw
                className={`text-gray-400 ${isLoading ? "animate-spin" : ""}`}
              />
            </Button>
          </div>
          <CardDescription>
            Click on a step to filter branches/tags by their processing status
          </CardDescription>
        </div>
      </CardHeader>
      <div className="flex flex-row gap-2 items-center justify-center">
        <ProcessingTab
          active={filter === ProcessStep.NotStarted}
          processStep={ProcessStep.NotStarted}
          data={branches}
          text="Initiating"
          onClick={() => setFilter(ProcessStep.NotStarted)}
        />

        <ArrowBigRightDash />
        <ProcessingTab
          data={branches}
          processStep={ProcessStep.SbomCreation}
          active={filter === ProcessStep.SbomCreation}
          onClick={() => setFilter(ProcessStep.SbomCreation)}
          text="SBOM Generation"
        />
        <ArrowBigRightDash />
        <ProcessingTab
          data={branches}
          processStep={ProcessStep.SbomIngest}
          active={filter === ProcessStep.SbomIngest}
          onClick={() => setFilter(ProcessStep.SbomIngest)}
          text="SBOM Ingestion"
        />
        <ArrowBigRightDash />
        <ProcessingTab
          data={branches}
          processStep={ProcessStep.Processed}
          active={filter === ProcessStep.Processed}
          onClick={() => setFilter(ProcessStep.Processed)}
          text="Analysis Complete"
        />
      </div>
      <div className="grid grid-cols-4 pt-6 w-3/5 gap-2 self-center pl-8">
        {branches
          ?.filter(
            (x) => ProcessStepOrder[x.processStep] >= ProcessStepOrder[filter]
          )
          .map((x) => (
            <BranchProcessingCard branch={x} filter={filter} />
          ))}
      </div>
    </Card>
  );
};

export default Processing;
