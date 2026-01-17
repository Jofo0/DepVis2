import { ProcessStep, type GetBranchesDto } from "@/types/branches";

const InitialProcessStepUtil = (data: GetBranchesDto | undefined) => {
  if (!data) {
    return ProcessStep.Processed;
  }

  const { initiated, sbomGenerated, sbomIngested, complete } = data;

  if (initiated > 0 && sbomGenerated === 0) {
    return ProcessStep.NotStarted;
  }
  if (sbomGenerated > 0 && sbomIngested === 0) {
    return ProcessStep.SbomCreation;
  }
  if (sbomIngested > 0 && complete === 0) {
    return ProcessStep.SbomIngest;
  }
  return ProcessStep.Processed;
};
export default InitialProcessStepUtil;
