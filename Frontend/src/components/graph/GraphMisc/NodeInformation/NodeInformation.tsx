import { CardDescription } from "@/components/ui/card";

type NodeInformationProps = {
  packageId?: string;
};

const NodeInformation = ({ packageId }: NodeInformationProps) => {
  if (!packageId)
    return (
      <div className="flex flex-col gap-4 border-2 rounded-2xl bg-white p-4 absolute bottom-10 right-10 z-1000">
        <CardDescription>Select a Node to see more information</CardDescription>
      </div>
    );

  return (
    <div className="flex flex-col gap-4 border-2 rounded-2xl bg-white p-4 absolute bottom-10 right-10 z-1000">
      selected Node: {packageId}
    </div>
  );
};

export default NodeInformation;
