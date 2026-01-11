import ProcessingCard from "@/components/cards/ProcessingCard";
import { Button } from "@/components/ui/button";
import { Card, CardHeader, CardContent } from "@/components/ui/card";
import { useGetProjectBranchesQuery } from "@/store/api/branchesApi";
import { ProcessStep } from "@/types/branches";
import { ArrowBigRightDash, Check, RefreshCcw } from "lucide-react";
import { useParams } from "react-router-dom";

const Processing = () => {
  const { id } = useParams<{ id: string }>();
  const {
    data: branches,
    isFetching: isLoading,
    refetch,
  } = useGetProjectBranchesQuery(id!);

  const firstStep = branches?.filter(
    (b) => b.processStep === ProcessStep.Created
  );

  const secondStep = branches?.filter(
    (b) => b.processStep === ProcessStep.SbomCreation
  );

  const thirdStep = branches?.filter(
    (b) => b.processStep === ProcessStep.SbomIngest
  );

  const fourthStep = branches?.filter(
    (b) => b.processStep === ProcessStep.Processed
  );

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
        <p className="border-2 rounded-2xl p-2 flex flex-row gap-2">
          Initiating {firstStep?.length === 0 && <Check />}
        </p>
        <ArrowBigRightDash />
        <p className="border-2 rounded-2xl p-2">SBOM Generation</p>
        <ArrowBigRightDash />
        <p className="border-2 rounded-2xl p-2">SBOM Ingestion</p>
        <ArrowBigRightDash />
        <p className="border-2 rounded-2xl p-2">Analysis Complete</p>
      </div>
      <CardContent className="flex flex-row justify-around w-full gap-4 items-center">
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
      </CardContent>
    </Card>
  );
};

export default Processing;
