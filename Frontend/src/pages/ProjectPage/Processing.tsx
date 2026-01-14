import { Button } from "@/components/ui/button";
import { Card, CardHeader } from "@/components/ui/card";
import { useGetProjectBranchesQuery } from "@/store/api/branchesApi";
import { ProcessStep, ProcessStepOrder } from "@/types/branches";
import { ArrowBigRightDash, RefreshCcw } from "lucide-react";
import { useParams } from "react-router-dom";
import ProcessingTab from "./Processing/ProcessingTab";
import { Skeleton } from "@/components/ui/skeleton";
import { useState } from "react";
import BranchProcessingCard from "./Processing/BranchProcessingCard";

const Processing = () => {
  const { id } = useParams<{ id: string }>();
  const {
    data,
    isFetching: isLoading,
    refetch,
  } = useGetProjectBranchesQuery(id!);
  const [filter, setFilter] = useState<ProcessStep>(ProcessStep.SbomIngest);
  const branches = data?.items;
  if (isLoading || !data) {
    return <Skeleton />;
  }

  return (
    <Card className="pb-10">
      <CardHeader>
        <div className="flex flex-row gap-2 items-center">
          Processing Statistics
          <Button variant={"ghost"} onClick={() => refetch()}>
            <RefreshCcw className="text-gray-400" />
          </Button>
        </div>
      </CardHeader>
      <div className="flex flex-row gap-2 items-center justify-center">
        <ProcessingTab
          filtered={data?.initiated}
          totalCount={data?.totalCount}
          text="Initiating"
          onClick={() => setFilter(ProcessStep.NotStarted)}
        />

        <ArrowBigRightDash />
        <ProcessingTab
          filtered={data?.sbomGenerated}
          totalCount={data?.totalCount}
          onClick={() => setFilter(ProcessStep.SbomCreation)}
          text="SBOM Generation"
        />
        <ArrowBigRightDash />
        <ProcessingTab
          filtered={data?.sbomIngested}
          totalCount={data?.totalCount}
          onClick={() => setFilter(ProcessStep.SbomIngest)}
          text="SBOM Ingestion"
        />
        <ArrowBigRightDash />
        <ProcessingTab
          filtered={data?.complete}
          totalCount={data?.totalCount}
          onClick={() => setFilter(ProcessStep.SbomIngest)}
          text="Analysis Complete"
        />
      </div>
      <div className="grid-cols-6 grid">
        {branches
          ?.filter(
            (x) => ProcessStepOrder[x.processStep] >= ProcessStepOrder[filter]
          )
          .map((x) => (
            <BranchProcessingCard branch={x} filter={filter} />
          ))}
      </div>
      {/* <CardContent className="flex flex-row justify-around w-full gap-4 items-center">
        <ProcessingCard
          isLoading={isLoading}
          branches={firstStep ?? []}
          header="Preparing for Processing"
          description="Branch/Tag/Commit is ready to be processed"
        />
        <ProcessingCard
          isLoading={isLoading}
          branches={secondStep ?? []}
          header="Creating SBOM"
          description="Branch/Tag/Commit is being processed and the SBOM is being created"
        />

        <ProcessingCard
          isLoading={isLoading}
          branches={thirdStep ?? []}
          header="Processing SBOM"
          description="The SBOM is being processed and the reports are being generated"
        />

        <ProcessingCard
          isLoading={isLoading}
          branches={fourthStep ?? []}
          header="Finished Processing"
          description="Branch/Tag/Commit has finished processing and reports are ready"
        />
      </CardContent> */}
    </Card>
  );
};

export default Processing;
