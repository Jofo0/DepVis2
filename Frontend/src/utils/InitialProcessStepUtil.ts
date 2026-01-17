import { ProcessStep, type GetBranchesDto } from "@/types/branches";

const InitialProcessStepUtil = (data: GetBranchesDto | undefined) => {
  if (!data) {
    return ProcessStep.Processed;
  }

  const { initiated, sbomGenerated, sbomIngested, complete, items } = data;

  if (
    (initiated > 0 ||
      items.filter((x) => x.processStep === ProcessStep.NotStarted).length >=
        0) &&
    sbomGenerated === 0 &&
    items.filter((x) => x.processStep === ProcessStep.SbomCreation).length === 0
  ) {
    return ProcessStep.NotStarted;
  }
  if (
    (sbomGenerated > 0 ||
      items.filter((x) => x.processStep === ProcessStep.SbomCreation).length >=
        0) &&
    sbomIngested === 0 &&
    items.filter((x) => x.processStep === ProcessStep.SbomIngest).length === 0
  ) {
    return ProcessStep.SbomCreation;
  }
  if (
    (sbomIngested > 0 ||
      items.filter((x) => x.processStep === ProcessStep.SbomIngest).length >=
        0) &&
    complete === 0 &&
    items.filter((x) => x.processStep === ProcessStep.Processed).length === 0
  ) {
    return ProcessStep.SbomIngest;
  }
  return ProcessStep.Processed;
};
export default InitialProcessStepUtil;
