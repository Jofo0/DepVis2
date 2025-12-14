import { CardDescription } from "@/components/ui/card";
import { X } from "lucide-react";
import Information from "./Components/Information";

type NodeInformationProps = {
  packageId?: string;
  onClose?: () => void;
};

const NodeInformation = ({ packageId, onClose }: NodeInformationProps) => {
  return (
    <div
      className={`flex ${
        packageId ? "w-3xl h-5/12" : "w-80 h-16"
      } flex-col gap-4 border-2 rounded-2xl bg-white p-4 absolute bottom-10 right-10 z-1000 transition-all duration-500 ease-in-out`}
    >
      {packageId ? (
        <>
          <div className="flex flex-row justify-between text-muted-foreground ">
            <CardDescription className="text-lg">
              Package Information
            </CardDescription>
            <X onClick={onClose} />
          </div>
          <Information packageId={packageId} />
        </>
      ) : (
        <CardDescription className="text-lg justify-center items-center flex">
          Select a Node to see more information
        </CardDescription>
      )}
    </div>
  );
};

export default NodeInformation;
