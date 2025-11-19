import ProcessingCard from "@/components/cards/ProcessingCard";
import { Card, CardHeader, CardContent } from "@/components/ui/card";
import { useGetProjectBranchesQuery } from "@/store/api/projectsApi";
import { ProcessStep } from "@/types/branches";
import { useParams } from "react-router-dom";

const Processing = () => {
  const { id } = useParams<{ id: string }>();
  const { data: branches, isLoading } = useGetProjectBranchesQuery(id!);
  return (
    <Card className="pb-10">
      <CardHeader>Processing Statistics</CardHeader>
      <CardContent className="flex flex-row justify-around w-full gap-4 items-center">
        <ProcessingCard
          isLoading={isLoading}
          branches={
            branches?.filter((b) => b.processStep === ProcessStep.Created) ?? []
          }
          header="Preparing for Processing"
          description="Branch/Tag/Commit is ready to be processed"
        />
        <ProcessingCard
          isLoading={isLoading}
          branches={
            branches?.filter(
              (b) => b.processStep === ProcessStep.SbomCreation
            ) ?? []
          }
          header="Creating SBOM"
          description="Branch/Tag/Commit is being processed and the SBOM is being created"
        />

        <ProcessingCard
          isLoading={isLoading}
          branches={
            branches?.filter((b) => b.processStep === ProcessStep.SbomIngest) ??
            []
          }
          header="Processing SBOM"
          description="The SBOM is being processed and the reports are being generated"
        />

        <ProcessingCard
          isLoading={isLoading}
          branches={
            branches?.filter((b) => b.processStep === ProcessStep.Processed) ??
            []
          }
          header="Finished Processing"
          description="Branch/Tag/Commit has finished processing and reports are ready"
        />
      </CardContent>
    </Card>
  );
};

export default Processing;
